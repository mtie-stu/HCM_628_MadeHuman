using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MadeHuman_Server.Model.User_Task
{
    public enum TaskTypeUser
    {
        Picker,
        Checker,
        Packer,
        Dispatcher
    }
    public class UsersTasks
    {
        [Key]
        public Guid Id { get; set; }

     

        
        public TaskTypeUser TaskType { get; set; }

       
        public DateTime WorkDate { get; set; }

        [Required]
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public AppUser User { get; set; }

        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }

        public TimeSpan? BreakDuration { get; set; }
        public TimeSpan? OvertimeDuration { get; set; }

        public bool IsCompleted { get; set; } = false;

        public string? Note { get; set; }
        public Guid PartTimeId { get; set; }
        [ForeignKey(nameof(PartTimeId))]
        public PartTime PartTimes { get; set; }
    }
}
