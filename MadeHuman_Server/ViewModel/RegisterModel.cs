using System.ComponentModel.DataAnnotations;

namespace MadeHuman_Server.ViewModel
{
    public class RegisterModel
    {
        [Required] public string? Name { get; set; }
        [Required][EmailAddress] public string? Email { get; set; }
        public string? SĐT { get; set; }
        [Required] public string? DiaChi { get; set; }
        [Required] public string? Password { get; set; }
        [Required]public string? PasswordConfirm { get; set; }
    }
}
