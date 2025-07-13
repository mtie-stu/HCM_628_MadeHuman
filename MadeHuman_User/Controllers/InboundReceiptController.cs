using Madehuman_Share.ViewModel.Inbound.InboundReceipt;
using MadeHuman_User.ServicesTask.Services.InboundService;
using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_User.Controllers
{
    public class InboundReceiptController : Controller
    {
        private readonly IInboundReceiptService _inboundReceiptService;

        public InboundReceiptController(IInboundReceiptService inboundReceiptService)
        {
            _inboundReceiptService = inboundReceiptService;
        }

        public async Task<IActionResult> Index()
        {
            var receipts = await _inboundReceiptService.GetAllAsync();
            return View(receipts);
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
