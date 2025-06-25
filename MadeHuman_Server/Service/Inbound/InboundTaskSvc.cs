using MadeHuman_Server.Data;
using MadeHuman_Server.Model.Inbound;
using MadeHuman_Server.Model.WareHouse;
using Madehuman_Share.ViewModel.Inbound;
using Microsoft.EntityFrameworkCore;

namespace MadeHuman_Server.Service.Inbound
{
    public interface IInboundTaskSvc
    {
        Task<InboundTasks> CreateInboundTaskAsync(CreateInboundTaskViewModel vm);
    }
    public class InboundTaskSvc : IInboundTaskSvc
    {
        private readonly ApplicationDbContext _context;

        public InboundTaskSvc(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<InboundTasks> CreateInboundTaskAsync(CreateInboundTaskViewModel vm)
        {
            // 1. Tìm phiếu nhập
            var receipt = await _context.InboundReceipt
                .Include(r => r.InboundReceiptItems)
                .ThenInclude(i => i.ProductSKUs)
                .FirstOrDefaultAsync(r => r.Id == vm.InboundReceiptId);

            if (receipt == null)
                throw new ArgumentException("InboundReceiptId không tồn tại.");

            if (receipt.Status != StatusReceipt.Confirmed)
                throw new InvalidOperationException("InboundReceipt phải ở trạng thái đã xác nhận.");

            // 2. Tạo InboundTask
            var taskId = Guid.NewGuid();
            var task = new InboundTasks
            {
                Id = taskId,
                CreateBy = vm.CreateBy,
                CreateAt = DateTime.UtcNow,
                Status = Status.Created,
                InboundReceiptId = vm.InboundReceiptId,
            };

            _context.InboundTasks.Add(task);

            // 3. Tìm danh sách kho trống
            var availableWarehouses = await _context.WarehouseLocations
                .Where(w => w.StatusWareHouse == StatusWareHouse.Empty)
                .Take(receipt.InboundReceiptItems.Count) // đảm bảo đủ số lượng
                .ToListAsync();

            if (availableWarehouses.Count < receipt.InboundReceiptItems.Count)
                throw new InvalidOperationException("Không đủ vị trí kho trống để chứa các lô hàng.");

            // 4. Tạo ProductBatches và gán kho
            int index = 0;
            foreach (var item in receipt.InboundReceiptItems)
            {
                var warehouse = availableWarehouses[index++];

                var productBatch = new ProductBatches
                {
                    Id = Guid.NewGuid(),
                    Quantity = item.Quantity,
                    ProductSKUId = item.ProductSKUId,
                    ProductSKU = item.ProductSKUs?.SKU,
                    StatusProductBatches = StatusProductBatches.UnStored,
                    InboundTaskId = taskId,
                    WarehouseLocationId = warehouse.Id
                };

                _context.ProductBatches.Add(productBatch);

                // Đánh dấu kho đã được đặt
                warehouse.StatusWareHouse = StatusWareHouse.Booked;
                _context.WarehouseLocations.Update(warehouse);
            }

            await _context.SaveChangesAsync();

            return task;
        }



    }
}
