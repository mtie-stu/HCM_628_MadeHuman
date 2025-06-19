namespace MadeHuman_Admin.Models
{
    public class AccountViewModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string State { get; set; }
    }
    public class AccountFilterViewModel
    {
        public string? SearchTerm { get; set; }
        public string? RoleFilter { get; set; }

        public List<AccountViewModel> Accounts { get; set; } = new();
    }
}
