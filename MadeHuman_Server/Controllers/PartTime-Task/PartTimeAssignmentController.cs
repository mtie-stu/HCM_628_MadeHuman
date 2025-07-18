using MadeHuman_Server.Service.UserTask;
using Madehuman_User.ViewModel.PartTime_Task;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/parttime-assignment")]
public class PartTimeAssignmentController : ControllerBase
{
    private readonly IPartTimeAssignmentService _service;

    public PartTimeAssignmentController(IPartTimeAssignmentService service)
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
    public async Task<IActionResult> Create(PartTimeAssignmentViewModel model)
    {
        var result = await _service.CreateAsync(model);
        return Ok(result);
    }

    [HttpPut]
    public async Task<IActionResult> Update(PartTimeAssignmentViewModel model)
    {
        var result = await _service.UpdateAsync(model);
        return Ok(result);
    }

    [HttpGet("{companyId}")]
    public IActionResult GetByCompanyId(Guid companyId)
    {
        var result = _service.GetByCompanyId(companyId);
        return Ok(result);
    }
}
