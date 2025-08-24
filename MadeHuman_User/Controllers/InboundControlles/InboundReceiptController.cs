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
            int pageSize = 6; // ✅ đổi từ 10 -> 6

            var query = await _inboundReceiptService.GetAllAsync();

            //if (!string.IsNullOrEmpty(zone))
            //    query = query.Where(x => x.Zone == zone).ToList();

            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(x => x.TaskCode != null && x.TaskCode.Contains(searchTerm)).ToList();

            int totalItems = query.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var data = query
                .OrderByDescending(x => x.CreateAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.SelectedZone = zone;
            ViewBag.SearchTerm = searchTerm;

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
