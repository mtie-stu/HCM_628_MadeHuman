using MadeHuman_Share.ViewModel;
using MadeHuman_User.Services.IServices;

namespace MadeHuman_User.Services
{
    public class LoginService : ILoginService
    {
        private readonly List<LoginModel> staticUsers = new List<LoginModel>
        {
            new LoginModel { EmailOrID = "admin01", Password = "Admin@123" },
            new LoginModel { EmailOrID = "admin@example.com", Password = "Admin@123" },
            new LoginModel { EmailOrID = "user01", Password = "User@123" },
            new LoginModel { EmailOrID = "user@example.com", Password = "User@123" }
        };

        public bool ValidateUser(string emailOrID, string password)
        {
            return staticUsers.Any(u =>
                u.EmailOrID.Equals(emailOrID, StringComparison.OrdinalIgnoreCase) &&
                u.Password == password
            );
        }
    }
}
