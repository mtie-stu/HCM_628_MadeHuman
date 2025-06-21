using MadeHuman_Server.Model;
using Madehuman_Share.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MadeHuman_Server.Service
{
    public interface IUserRegistrationService
    {
        Task<List<string>> RegisterPartTimeUsersAsync(BulkRegisterModel model);
    }

    public class UserRegistrationSvc : IUserRegistrationService
    {
        private readonly UserManager<AppUser> _userManager;

        public UserRegistrationSvc(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<List<string>> RegisterPartTimeUsersAsync(BulkRegisterModel model)
        {
            var createdUsers = new List<string>();

            // Validate mật khẩu đầu vào
            if (string.IsNullOrWhiteSpace(model.DefaultPassword))
            {
                // Tự tạo mật khẩu mặc định nếu không có
                model.DefaultPassword = $"MadeHuman@123";
                Console.WriteLine($"⚠️ Mật khẩu mặc định trống. Hệ thống tự tạo: {model.DefaultPassword= "MadeHuman@123"}");
            }

            // Lấy danh sách username bắt đầu bằng "PT_"
            var existingUsers = await _userManager.Users
                .Where(u => u.UserName.StartsWith("PT_"))
                .Select(u => u.UserName)
                .ToListAsync();

            // Tìm số lớn nhất trong danh sách
            int maxNumber = 0;
            foreach (var username in existingUsers)
            {
                var parts = username.Split('_');
                if (parts.Length == 2 && int.TryParse(parts[1], out int number))
                {
                    if (number > maxNumber)
                        maxNumber = number;
                }
            }

            // Tạo người dùng mới
            for (int i = 1; i <= model.Quantity; i++)
            {
                int userNumber = maxNumber + i;
                string code = $"PT_{userNumber:D3}";
                string email = $"{code}@MadeHuman.com";

                var user = new AppUser
                {
                    UserName = code,
                    NormalizedUserName = code.ToUpper(),
                    Email = email,
                    NormalizedEmail = email.ToUpper(),
                    EmailConfirmed = true,
                    Status = (AppUser.UserStatus)model.UserStatus
                };

                var result = await _userManager.CreateAsync(user, model.DefaultPassword);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "PartTime");
                    createdUsers.Add(email);
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    Console.WriteLine($"❌ Không tạo được user {email}: {errors}");
                }
            }

            return createdUsers;
        }

    }
}
