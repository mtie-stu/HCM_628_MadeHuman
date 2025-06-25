using MadeHuman_Server.Service.UserTask;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_Server.Controllers.PartTime_Task
{
    [ApiController]
    [Route("api/company")]
    public class CompanyController : ControllerBase
    {
        private readonly IPartTimeCompanyService _companyService;

        public CompanyController(IPartTimeCompanyService companyService)
        {
            _companyService = companyService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddCompany(string name, string? address)
        {
            var result = await _companyService.AddCompanyAsync(name, address);
            return Ok(result);
        }

        [HttpPut("{id}/inactive")]
        public async Task<IActionResult> SetInactive(Guid id)
        {
            var success = await _companyService.SetActiveAsync(id);
            if (!success) return NotFound();
            return Ok("Cập nhật trạng thái thành công");
        }
    }

}
