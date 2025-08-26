// File: PickTaskServices.cs

using MadeHuman_Server.Data;
using MadeHuman_Server.Model.Inbound;
using MadeHuman_Server.Model.Outbound;
using MadeHuman_Server.Service.UserTask;
using Madehuman_Share.ViewModel.Outbound;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MadeHuman_Server.Service.Outbound
{
    public interface IPickTaskServices
    {
        Task<List<PickTaskViewModelForIndexView>> GetPickTasksByUserTaskIdAsync();
        Task<Guid?> AssignPickTaskToCurrentUserAsync();
        Task<List<string>> ValidatePickTaskScanAsync(ScanPickTaskValidationRequest request);
        Task<ConfirmPickResult> ConfirmPickDetailToBasketAsync(Guid pickTaskId, Guid pickTaskDetailId, Guid basketId);
        Task<PickTaskFullViewModel?> GetPickTaskDetailByIdAsync(Guid pickTaskId);
        Task<List<string>> AssignBasketToOutboundTaskAsync(Guid basketId, Guid outboundTaskId);
    }

    public class PickTaskServices : IPickTaskServices
    {
        private readonly ICheckTaskServices _checkTaskService;
        private readonly ILogger<PickTaskServices> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IUserTaskSvc _usertaskservice;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PickTaskServices(ApplicationDbContext dbContext, IUserTaskSvc userTaskSvc, IHttpContextAccessor httpContextAccessor, ICheckTaskServices checkTaskService, ILogger<PickTaskServices> logger)
        {
            _context = dbContext;
            _usertaskservice = userTaskSvc;
            _httpContextAccessor = httpContextAccessor;
            _checkTaskService = checkTaskService;
            _logger = logger;
        }

        //public async Task<PickTaskFullViewModel?> AssignPickTaskToCurrentUserAsync()
        //{
        //    var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        //    if (string.IsNullOrEmpty(userId))
        //        throw new UnauthorizedAccessException("❌ Không xác định được người dùng hiện tại.");

        //    var userTaskId = await _usertaskservice.GetUserTaskIdAsync(userId, DateOnly.FromDateTime(DateTime.UtcNow));
        //    if (userTaskId == null)
        //        throw new InvalidOperationException("❌ Không tìm thấy phân công công việc hôm nay cho người dùng.");

        //    var task = await _context.PickTasks
        //        .Include(p => p.PickTaskDetails)
        //        .Where(p => p.UsersTasksId == null || p.UsersTasksId == Guid.Empty)
        //        .OrderBy(p => p.CreateAt)
        //        .FirstOrDefaultAsync();

        //    if (task == null)
        //        return null;

        //    task.UsersTasksId = userTaskId;
        //    task.Status = StatusPickTask.Recived;
        //    await _context.SaveChangesAsync();

        //    return new PickTaskFullViewModel
        //    {
        //        Id = task.Id,
        //        CreateAt = task.CreateAt,
        //        FinishAt = task.FinishAt,
        //        Status = (StatusPickTaskvm)task.Status,
        //        UsersTasksId = task.UsersTasksId,
        //        BasketId = _context.Baskets.FirstOrDefault(b => b.OutBoundTaskId == task.OutboundTaskId)?.Id,
        //        Details = task.PickTaskDetails.Select(d => new PickTaskFullViewModel.PickTaskDetailItem
        //        {
        //            Id = d.Id,
        //            Quantity = d.Quantity,
        //            ProductSKUId = d.ProductSKUId,
        //            WarehouseLocationId = d.WarehouseLocationId
        //        }).ToList()
        //    };
        //}


        public async Task<Guid?> AssignPickTaskToCurrentUserAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("❌ Không xác định được người dùng hiện tại.");

            var userTaskId = await _usertaskservice.GetUserTaskIdAsync(userId, DateOnly.FromDateTime(DateTime.UtcNow));
            if (userTaskId == null)
                throw new InvalidOperationException("❌ Không tìm thấy phân công công việc hôm nay cho người dùng.");

            var task = await _context.PickTasks
                .Where(p => p.UsersTasksId == null || p.UsersTasksId == Guid.Empty)
                .OrderBy(p => p.CreateAt)
                .FirstOrDefaultAsync();

            if (task == null)
                return null;

            task.UsersTasksId = userTaskId;
            task.Status = StatusPickTask.Recived;
            await _context.SaveChangesAsync();

            return task.Id;
        }

        public async Task<List<string>> ValidatePickTaskScanAsync(ScanPickTaskValidationRequest request)
        {
            var task = await _context.PickTasks.Include(t => t.PickTaskDetails).FirstOrDefaultAsync(t => t.Id == request.PickTaskId);
            if (task == null) return new() { "❌ Không tìm thấy PickTask." };

            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return new() { "❌ Không xác định được người dùng." };

            var userTaskId = await _usertaskservice.GetUserTaskIdAsync(userId, DateOnly.FromDateTime(DateTime.UtcNow));
            if (userTaskId == null) return new() { "❌ Không tìm thấy phân công công việc hôm nay." };

            if (task.UsersTasksId != userTaskId) return new() { "❌ PickTask này không được gán cho bạn." };

            var detail = task.PickTaskDetails.FirstOrDefault(d => d.Id == request.PickTaskDetailId);
            if (detail == null) return new() { "❌ Không tìm thấy chi tiết Pick." };
            if (detail.IsPicked) return new() { "❌ Chi tiết này đã hoàn tất." };

            if (string.IsNullOrEmpty(request.WareHouseLocation)) return new() { "❌ Vị trí không hợp lệ." };

            var locationId = await _context.WarehouseLocations.Where(t => t.NameLocation == request.WareHouseLocation).Select(t => t.Id).FirstOrDefaultAsync();
            if (locationId != detail.WarehouseLocationId) return new() { "❌ Vị trí không khớp." };

            var sku = await _context.ProductSKUs.Where(p => p.Id == detail.ProductSKUId).Select(p => p.SKU).FirstOrDefaultAsync();
            if (!string.Equals(request.SKU, sku, StringComparison.OrdinalIgnoreCase))
                return new() { $"❌ SKU không khớp. Hệ thống: {sku}, bạn nhập: {request.SKU}" };

            detail.QuantityPicked += 1;
            await _context.SaveChangesAsync();

            return new() { $"✅ Đã pick {detail.QuantityPicked}/{detail.Quantity}." };
        }

        public async Task<ConfirmPickResult> ConfirmPickDetailToBasketAsync(Guid pickTaskId, Guid pickTaskDetailId, Guid basketId)
        {
            var result = new ConfirmPickResult();

            var task = await _context.PickTasks.Include(t => t.PickTaskDetails).FirstOrDefaultAsync(t => t.Id == pickTaskId);
            if (task == null) return Fail("❌ Không tìm thấy nhiệm vụ.");

            var detail = task.PickTaskDetails.FirstOrDefault(d => d.Id == pickTaskDetailId);
            if (detail == null) return Fail("❌ Không tìm thấy chi tiết nhiệm vụ.");

            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) return Fail("❌ Không xác định được người dùng.");

            var userTaskId = await _usertaskservice.GetUserTaskIdAsync(userId, DateOnly.FromDateTime(DateTime.UtcNow));
            if (userTaskId == null) return Fail("❌ Không tìm thấy phân công công việc hôm nay.");

            if (!detail.IsPicked)
            {

                var assignResult = await AssignBasketToOutboundTaskAsync(basketId, task.OutboundTaskId);
                if (assignResult.Any()) return Fail(assignResult.ToArray());

                detail.IsPicked = true;

                var inventory = await _context.Inventory.FirstOrDefaultAsync(i =>
                    i.ProductSKUId == detail.ProductSKUId &&
                    i.WarehouseLocationId == detail.WarehouseLocationId);

                if (inventory == null || inventory.StockQuantity < detail.Quantity)
                    return Fail("❌ Không đủ tồn kho để hoàn tất pick. Vui lòng nhờ staff hỗ trợ.");

                inventory.StockQuantity -= detail.Quantity;
                inventory.LastUpdated = DateTime.UtcNow;

                _context.InventoryLogs.Add(new InventoryLogs
                {
                    Id = Guid.NewGuid(),
                    InventoryId = inventory.Id,
                    StockQuantity = inventory.StockQuantity,
                    ChangeQuantity = -detail.Quantity,
                    RemainingQuantity = inventory.StockQuantity,
                    ActionInventoryLogs = ActionInventoryLogs.Take,
                    ChangeBy = userTaskId.ToString(),
                    Time = DateTime.UtcNow
                });

                result.Messages.Add("✅ Đã xác nhận rổ và hoàn tất chi tiết pick.");
            }
            else
            {
                result.Messages.Add("✅ Chi tiết này đã hoàn tất rồi.");
            }

            // Kiểm tra toàn bộ PickTask
            if (task.PickTaskDetails.All(d => d.IsPicked))
            {
                task.Status = StatusPickTask.Finished;
                task.FinishAt = DateTime.UtcNow;

                var totalQty = task.PickTaskDetails.Sum(d => d.Quantity);
                var userTask = await _context.UsersTasks.FirstOrDefaultAsync(u => u.Id == userTaskId);
                if (userTask != null)
                {
                    userTask.TotalKPI += totalQty;
                    userTask.HourlyKPIs += totalQty;
                }

                await _checkTaskService.CreateCheckTaskAsync(task.OutboundTaskId);
                result.IsPickTaskFinished = true;
            }
            else
            {
                result.IsPickTaskFinished = false;

                var next = task.PickTaskDetails
                    .Where(d => !d.IsPicked)
                    .Select(d => new
                    {
                        d.Id,
                        d.ProductSKUId,
                        d.Quantity,
                        d.WarehouseLocationId
                    })
                    .FirstOrDefault();

                result.NextDetail = next;
            }

            await _context.SaveChangesAsync();
            return result;

            ConfirmPickResult Fail(params string[] msgs) => new() { Messages = msgs.ToList(), IsPickTaskFinished = false };
        }

        public async Task<List<string>> AssignBasketToOutboundTaskAsync(Guid basketId, Guid outboundTaskId)
        {
            var basket = await _context.Baskets.FindAsync(basketId);
            if (basket == null) return new() { "❌ Không tìm thấy giỏ." };

            if (basket.OutBoundTaskId != null && basket.OutBoundTaskId != outboundTaskId)
                return new() { "❌ Giỏ này đã được sử dụng cho nhiệm vụ khác. Vui lòng quét giỏ khác." };

            if (basket.OutBoundTaskId == null)
            {
                basket.OutBoundTaskId = outboundTaskId;
                basket.Status = StatusBaskets.Selected;
                _context.Baskets.Update(basket);
                await _context.SaveChangesAsync();
            }

            return new();
        }

        public async Task<List<PickTaskViewModelForIndexView>> GetPickTasksByUserTaskIdAsync()
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId)) throw new UnauthorizedAccessException("❌ Không xác định được người dùng hiện tại.");

            var userTaskId = await _usertaskservice.GetUserTaskIdAsync(userId, DateOnly.FromDateTime(DateTime.UtcNow));
            if (userTaskId == null) throw new InvalidOperationException("❌ Không tìm thấy phân công công việc hôm nay cho người dùng.");

            var tasks = await _context.PickTasks
                .Include(p => p.OutboundTaskItems)
                .Include(p => p.PickTaskDetails)
                .Where(p => p.UsersTasksId == userTaskId)
                .ToListAsync();

            return tasks.Select(task => new PickTaskViewModelForIndexView
            {
                Id = task.Id,
                CreateAt = task.CreateAt,
                FinishAt = task.FinishAt,
                Status = task.Status.ToString(),
                OutboundTaskId = task.OutboundTaskId,
                TotalQuantity = task.PickTaskDetails.Sum(d => d.Quantity)
            }).ToList();
        }
        public async Task<PickTaskFullViewModel?> GetPickTaskDetailByIdAsync(Guid pickTaskId)
        {
            var outboundTaskId = await _context.PickTasks
                .Where(b => b.Id == pickTaskId)
                .Select(b => b.OutboundTaskId)
                .FirstOrDefaultAsync();

            var basketid = await _context.Baskets
                .Where(b => b.OutBoundTaskId == outboundTaskId)
                .Select(b => (Guid?)b.Id)
                .FirstOrDefaultAsync();

            var entity = await _context.PickTasks
                .Include(p => p.PickTaskDetails)
                .AsNoTracking()
                .Where(p => p.Id == pickTaskId)
                .ToListAsync();

            var data = entity.Select(p => new PickTaskFullViewModel
            {
                Id = p.Id,
                CreateAt = p.CreateAt,
                Status = (StatusPickTaskvm)Enum.Parse(typeof(StatusPickTaskvm), p.Status.ToString()),
                UsersTasksId = p.UsersTasksId,
                BasketId = basketid,
                Details = p.PickTaskDetails
                    .Where(d => !d.IsPicked) // ✅ chỉ lấy các detail chưa được pick
                    .Select(d => new PickTaskFullViewModel.PickTaskDetailItem
                    {
                        Id = d.Id,
                        Quantity = d.Quantity,
                        WarehouseLocationId = d.WarehouseLocationId,
                        ProductSKUId = d.ProductSKUId
                    })
                    .OrderBy(d => d.Id)
                    .ToList()
            }).FirstOrDefault();

            return data;
        }



    }
}


//🔄 1.Nhận nhiệm vụ Pick: AssignPickTaskToCurrentUserAsync()
//✅ Xác định UserId từ token và lấy UsersTasksId theo ngày hiện tại.

//🔍 Tìm PickTask chưa được gán người dùng.

//🖋️ Gán UsersTasksId, đổi trạng thái sang Recived.

//Trả về PickTaskFullViewModel gồm:

//PickTaskId, CreateAt, Status

//BasketId (nếu đã có giỏ)

//Danh sách PickTaskDetails (mỗi detail gồm: Id, Quantity, SKUId, LocationId)

//📌 Giai đoạn này là khởi tạo và hiển thị nhiệm vụ trên View ScanPickDetails.

//📦 2. Quét SKU và xác nhận: ValidatePickTaskScanAsync(request)
//Xác thực:

//PickTask tồn tại, được gán đúng UserTask.

//PickTaskDetail tồn tại, chưa hoàn tất.

//SKU và vị trí đúng.

//Nếu đúng:

//Tăng QuantityPicked thêm 1.

//Nếu sai: trả lỗi để thông báo trên View.

//📌 Mỗi lần quét đúng thì tăng tiến dần cho tới khi đạt Quantity.

//🧺 3. Xác nhận đã cho hàng vào rổ: ConfirmPickDetailToBasketAsync(pickTaskId, pickTaskDetailId, basketId)
//Kiểm tra PickTaskDetail:

//Đã quét đủ QuantityPicked thì mới cho xác nhận.

//Kiểm tra BasketId:

//Nếu đã gán cho nhiệm vụ khác → báo lỗi.

//Nếu chưa gán → gán vào nhiệm vụ hiện tại.

//Đánh dấu PickTaskDetail là IsPicked = true.

//Trừ tồn kho từ Inventory.

//Ghi log vào InventoryLogs.

//✅ Nếu tất cả các PickTaskDetails đã hoàn tất:

//Cập nhật Status = Finished.

//Ghi FinishAt

//Cộng KPI cho Picker

//Tạo tiếp CheckTask cho nhiệm vụ kế tiếp (Kiểm hàng).

//📋 4. Lấy danh sách nhiệm vụ đã được gán: GetPickTasksByUserTaskIdAsync()
//Lấy tất cả các PickTask gán cho UsersTasksId hôm nay.

//Trả về danh sách view model gồm:

//Id, CreateAt, FinishAt, Status

//Tổng Quantity từ các PickTaskDetail.

//📌 Dùng để hiển thị danh sách nhiệm vụ Pick của tôi trong view Index.

//🔁 Hỗ trợ nội bộ: AssignBasketToOutboundTaskAsync
//Đảm bảo 1 Basket chỉ gán cho duy nhất 1 nhiệm vụ (OutboundTask).

//Nếu chưa gán → sẽ gán ngay và đổi trạng thái Basket thành Selected.

//✅ Tổng Kết Flow Thực Tế
//Giai đoạn	Mô tả	Giao diện
//1️⃣ Nhận nhiệm vụ	AssignPickTaskToCurrentUserAsync()	Index View → Scan View
//2️⃣ Quét SKU & vị trí	ValidatePickTaskScanAsync()	Scan View (1 SKU 1 lần)
//3️⃣ Xác nhận bỏ vào rổ	ConfirmPickDetailToBasketAsync()	Scan View (Quét Basket)
//4️⃣ Hoàn tất	Ghi tồn kho, cộng KPI, tạo CheckTask	Chuyển về Index View