using MadeHuman_Server.Model.Inbound;
using MadeHuman_Server.Service.Inbound;
using Madehuman_Share.ViewModel.Inbound;
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
            var receipts = await _inboundReciptService.GetAllAsync();
            return Ok(receipts);
        }

        // GET: api/InboundReceipt/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var receipt = await _inboundReciptService.GetByIdAsync(id);
            if (receipt == null)
                return NotFound();

            return Ok(receipt);
        }
    }
}
