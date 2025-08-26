using Madehuman_Share.ViewModel.Inbound.InboundReceipt;
using MadeHuman_User.ServicesTask.Services.InboundService;
using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_User.Controllers.InboundControlles
{
    public class InboundReceiptController : Controller
    {
        private readonly IInboundReceiptService _inboundReceiptService;

        public InboundReceiptController(IInboundReceiptService inboundReceiptService)
        {
            _inboundReceiptService = inboundReceiptService;
        }

        public async Task<IActionResult> Index(int page = 1, string zone = "", string searchTerm = "")
        {
            const int pageSize = 6; // bạn đang dùng 6
            var all = await _inboundReceiptService.GetAllAsync();

            // (Tuỳ chọn) lọc zone nếu cần
            // if (!string.IsNullOrWhiteSpace(zone))
            //     all = all.Where(x => x.Zone == zone).ToList();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                all = all.Where(x =>
                       (!string.IsNullOrEmpty(x.TaskCode) && x.TaskCode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    || x.Id.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var totalItems = all.Count;
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalItems / (double)pageSize));
            var currentPage = Math.Min(Math.Max(1, page), totalPages); // kẹp trong [1..totalPages]

            var data = all.OrderByDescending(x => x.CreateAt)
                          .Skip((currentPage - 1) * pageSize)
                          .Take(pageSize)
                          .ToList();

            // Cửa sổ trang (…)
            const int window = 2; // hiển thị 2 trang trước/sau
            var startPage = Math.Max(1, currentPage - window);
            var endPage = Math.Min(totalPages, currentPage + window);

            // Range hiển thị “X–Y / total”
            var from = totalItems == 0 ? 0 : ((currentPage - 1) * pageSize) + 1;
            var to = Math.Min(currentPage * pageSize, totalItems);

            ViewBag.CurrentPage = currentPage;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.StartPage = startPage;
            ViewBag.EndPage = endPage;
            ViewBag.SelectedZone = zone;
            ViewBag.SearchTerm = searchTerm;
            ViewBag.From = from;
            ViewBag.To = to;

            return View(data);
        }



        [HttpGet]
        public IActionResult Create()
        {
            var vm = new CreateInboundReceiptViewModel
            {
                CreateAt = DateTime.UtcNow,
                ReceivedAt = DateTime.UtcNow
            };

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateInboundReceiptViewModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var created = await _inboundReceiptService.CreateReceiptAsync(vm);

            if (!created)
            {
                ModelState.AddModelError("", "Tạo phiếu nhập thất bại");
                return View(vm);
            }

            TempData["Success"] = "Tạo phiếu nhập thành công.";
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            var receipt = await _inboundReceiptService.GetByIdAsync(id);
            if (receipt == null)
                return NotFound();

            return View(receipt);
        }
    }
}
