namespace MadeHuman_Admin.Models
{
    public class PartTimeRoleHistory
    {
        public DateTime Date { get; set; }
        public string PartTimeId { get; set; }
        public string Role { get; set; }
    }


    public class SetPartTimeRoleViewModel
    {
        public string PartTimeId { get; set; }
        public string Role { get; set; }
        public List<PartTimeRoleHistory> History { get; set; } = new();
    }

}
