using MadeHuman_Server.Model.Inbound;
using MadeHuman_Server.Service.Inbound;
using Madehuman_User.ViewModel.Inbound.InboundReceipt;
using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_Server.Controllers.Inbound
{
    [Route("api/[controller]")]
    [ApiController]
    public class InboundReceiptController : ControllerBase
    {
        private readonly IInboundReciptService _inboundReciptService;

        public InboundReceiptController(IInboundReciptService inboundReciptService)
        {
            _inboundReciptService = inboundReciptService;
        }

        // POST: api/InboundReceipt
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateInboundReceiptViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _inboundReciptService.CreateAsync(model);
            return Ok(result);
        }

        // GET: api/InboundReceipt
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var receipts = await _inboundReciptService.GetAllAsync(); // Trả về List<InboundReceipts>

            var result = receipts.Select(r => new InboundReceiptViewModel
            {
                Id = r.Id,
                CreateAt = r.CreateAt,
                ReceivedAt = r.ReceivedAt,
                Status = r.Status.ToString(), // enum → string

                TaskCode = r.InboundTasks?.Id.ToString(), // hoặc .TaskCode nếu có
                TaskStatus = r.InboundTasks?.Status.ToString(),

                ItemCount = r.InboundReceiptItems?.Count ?? 0
            });

            return Ok(result);
        }



        // GET: api/InboundReceipt/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var r = await _inboundReciptService.GetByIdAsync(id);
            if (r == null)
                return NotFound();

            var result = new InboundReceiptDetailViewModel
            {
                Id = r.Id,
                CreateAt = r.CreateAt,
                ReceivedAt = r.ReceivedAt,
                Status = r.Status.ToString(),
                TaskCode = r.InboundTasks?.Id.ToString(), // nếu có TaskCode thì đổi lại
                TaskStatus = r.InboundTasks?.Status.ToString(),

                Items = r.InboundReceiptItems?.Select(i => new InboundReceiptItemDetail
                {
                    ProductSKUId = i.ProductSKUId,
                    ProductSKUName = i.ProductSKUs?.SKU ?? "(Không có tên)", // 👈 lấy tên SKU nếu có
                    Quantity = i.Quantity
                }).ToList() ?? new()
            };

            return Ok(result);
        }


    }
}
