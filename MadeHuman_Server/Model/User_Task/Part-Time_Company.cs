using System.ComponentModel.DataAnnotations;

namespace MadeHuman_Server.Model.User_Task
{
    public enum StatusPart_Time_Company
    {
        Active,
        Inactive
    }
    public class Part_Time_Company
    {
        [Key]
        public Guid Id { get; set; } = default!;

        [Required]
        public string Name { get; set; } = string.Empty;

        public string? Address { get; set; }
        // ✅ Quan hệ 1-N đến PartTime
        public ICollection<PartTime> PartTimes { get; set; } = new List<PartTime>();
        public StatusPart_Time_Company Status { get; set; } 
        // Quan hệ 1-N: Một công ty có nhiều phân công
        public ICollection<PartTimeAssignment> Assignments { get; set; } = new List<PartTimeAssignment>();
    }
}
