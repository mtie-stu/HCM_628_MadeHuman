using MadeHuman_Server.Service.UserTask;
using Madehuman_User.ViewModel.PartTime_Task;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_Server.Controllers.PartTime_Task
{
    [ApiController]
    [Route("api/parttime")]
    public class PartTimeController : ControllerBase
    {
        private readonly IPartTimeService _service;

        public PartTimeController(IPartTimeService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _service.GetAllAsync();
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create(PartTimeViewModel model)
        {
            var result = await _service.CreateAsync(model);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update(PartTimeViewModel model)
        {
            var result = await _service.UpdateAsync(model);
            return Ok(result);
        }

        [HttpGet("{companyId}")]
        public IActionResult GetByCompanyId(Guid companyId)
        {
            var result =_service.GetByCompanyId(companyId);
            return Ok(result);
        }
    }



}
