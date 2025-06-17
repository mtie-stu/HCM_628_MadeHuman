using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MadeHuman_User.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Please enter your email or user ID.")]
        public string EmailOrID { get; set; }

        [Required(ErrorMessage = "Please enter your password.")]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        public string Password { get; set; }

        // ✅ Kiểm tra định dạng Email
        public bool IsEmail
        {
            get
            {
                if (string.IsNullOrWhiteSpace(EmailOrID))
                    return false;

                return Regex.IsMatch(EmailOrID,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase);
            }
        }

        // ✅ Kiểm tra mật khẩu có đúng định dạng không
        public bool IsValidPasswordFormat
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Password))
                    return false;

                // Ít nhất 8 ký tự, 1 chữ hoa, 1 số, 1 ký tự đặc biệt
                return Regex.IsMatch(Password,
                    @"^(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
            }
        }
    }
}
