using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MadeHuman_Server.Model.User_Task
{
    public class CheckInCheckOutLog
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string PartTimeId { get; set; }

        [Required]
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public AppUser User { get; set; }
 
        public DateTime Timestamp { get; set; }

        public bool IsCheckIn { get; set; }            // true = checkin, false = checkout

        public string? Note { get; set; }

        public Guid? UsersTasksId { get; set; }
        [ForeignKey(nameof(UsersTasksId))]
        public UsersTasks? UsersTasks { get; set; }
    }
}
