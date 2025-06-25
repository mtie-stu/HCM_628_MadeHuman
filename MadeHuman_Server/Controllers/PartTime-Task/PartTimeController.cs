using MadeHuman_Server.Service.UserTask;
using Madehuman_Share.ViewModel.PartTime_Task;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_Server.Controllers.PartTime_Task
{
    [ApiController]
    [Route("api/parttime")]
    public class PartTimeController : ControllerBase
    {
        private readonly IPartTimeService _partTimeService;

        public PartTimeController(IPartTimeService partTimeService)
        {
            _partTimeService = partTimeService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePartTimeViewModel dto)
        {
            var result = await _partTimeService.CreateAsync(dto.PartTimeId, dto.Name, dto.CCCD, dto.PhoneNumber, dto.CompanyId);
            return Ok(result);
        }
    }



}
