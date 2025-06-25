/*using MadeHuman_Server.Model.User_Task;
using MadeHuman_Server.Service.UserTask;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_Server.Controllers.PartTime_Task
{
    [ApiController]
    [Route("api/assignments")]
    public class PartTimeAssignmentController : ControllerBase
    {
        private readonly IPartTimeAssignmentService _service;

        public PartTimeAssignmentController(IPartTimeAssignmentService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PartTimeAssignment model)
        {
            var result = await _service.CreateAssignmentAsync(model);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _service.GetByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _service.GetAllAsync();
            return Ok(list);
        }

        [HttpPut("{id}/confirm")]
        public async Task<IActionResult> Confirm(Guid id)
        {
            var success = await _service.ConfirmAssignmentAsync(id);
            return success ? Ok("Confirmed") : NotFound();
        }

    }

}
*/