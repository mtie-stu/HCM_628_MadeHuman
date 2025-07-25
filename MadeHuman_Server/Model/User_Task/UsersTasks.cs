using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using MadeHuman_Server.Model.Inbound;
using System.Text.Json.Serialization;
using MadeHuman_Server.Model.Outbound;

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
        public Guid Id { get; set; } = default!;
 
        public TaskTypeUser TaskType { get; set; }

       
        public DateTime WorkDate { get; set; } = default!;

        [Required]
        public string UserId { get; set; } = string.Empty;
        [ForeignKey(nameof(UserId))]
        public AppUser User { get; set; }

        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }

        public TimeSpan? BreakDuration { get; set; }
        public TimeSpan? OvertimeDuration { get; set; }

        public bool IsCompleted { get; set; } = false;

        public string? Note { get; set; }
        public Guid? PartTimeId { get; set; }
        [ForeignKey(nameof(PartTimeId))]
        public PartTime PartTimes { get; set; }


        [JsonIgnore]
        public ICollection<InboundTasks> InboundTasks { get; set; }
        [JsonIgnore]
        public ICollection<PickTasks> PickTasks { get; set; }
        [JsonIgnore]
        public ICollection<RefillTasks> RefillTasks { get; set; }
        [JsonIgnore]
        public ICollection<CheckTasks> CheckTasks { get; set; }
        [JsonIgnore]
        public ICollection<PackTask> PackTask { get; set; }
        [JsonIgnore]
        public ICollection<DispatchTasks> DispatchTasks { get; set; }
        public int TotalKPI { get; set; } = 0;
        public int HourlyKPIs { get; set; } = 0;
    }
}
