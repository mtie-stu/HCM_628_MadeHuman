using MadeHuman_Server.Service;
using Madehuman_User.ViewModel;
using Microsoft.AspNetCore.Mvc;

namespace MadeHuman_Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserRegistrationController : ControllerBase
    {
        private readonly IUserRegistrationService _registrationService;

        public UserRegistrationController(IUserRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        /// <summary>
        /// Tạo tài khoản PartTime hàng loạt theo số lượng chỉ định.
        /// </summary>
        /// <param name="model">Thông tin số lượng và cấu hình user</param>
        /// <returns>Danh sách email đã tạo</returns>
        [HttpPost("bulk-register/parttime")]
        public async Task<IActionResult> BulkRegisterPartTime([FromBody] BulkRegisterModel model)
        {
            if (model.Quantity <= 0)
            {
                return BadRequest(new { message = "Số lượng tài khoản phải lớn hơn 0." });
            }

            var emails = await _registrationService.RegisterPartTimeUsersAsync(model);

            return Ok(new
            {
                message = $"Tạo thành công {emails.Count} tài khoản.",
                emails
            });
        }
    }
}
