using Microsoft.AspNetCore.Identity;

namespace MadeHuman_Server.Model
{
    public class AppUser : IdentityUser
    {
        public string? Name { get; set; }
        public string? Image { get; set; }
    }
}
