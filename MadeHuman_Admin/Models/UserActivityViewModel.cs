namespace MadeHuman_Admin.Models
{
    public class UserActivityViewModel
    {
        public string UserId { get; set; }
        public string Activity { get; set; } // "CheckIn" or "CheckOut"
        public DateTime? CheckIn { get; set; }
        public string Role { get; set; }
        public double? TotalHours { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
    }
}
