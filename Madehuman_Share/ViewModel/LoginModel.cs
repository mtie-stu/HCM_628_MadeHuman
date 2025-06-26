using System.ComponentModel.DataAnnotations;

namespace Madehuman_Share.ViewModel
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Email hoặc ID không được để trống")]
        public string EmailOrID { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d]).{8,}$",
            ErrorMessage = "Mật khẩu phải có ít nhất 8 ký tự, bao gồm chữ hoa, chữ thường, số và ký tự đặc biệt")]
        public string Password { get; set; } = string.Empty;
    }
}
