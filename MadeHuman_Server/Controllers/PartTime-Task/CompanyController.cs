using MadeHuman_Server.Service.UserTask;
using Madehuman_User.ViewModel.PartTime_Task;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_Server.Controllers.PartTime_Task
{
    [ApiController]
    [Route("api/company")]
    public class CompanyController : ControllerBase
    {
        private readonly IPartTimeCompanyService _service;
        public CompanyController(IPartTimeCompanyService service)
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
        public async Task<IActionResult> Create(PartTimeCompanyViewModel model)
        {
            var result = await _service.CreateAsync(model);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> Update(PartTimeCompanyViewModel model)
        {
            var result = await _service.UpdateAsync(model);
            return Ok(result);
        }
    }

}
