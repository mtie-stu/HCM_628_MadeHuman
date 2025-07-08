using MadeHuman_Server.Model.User_Task;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

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

    public bool IsCheckIn { get; set; } // true = checkin, false = checkout

    public bool IsOvertime { get; set; } // ✅ Mới: true nếu là log tăng ca

    public string? Note { get; set; }

    public Guid? UsersTasksId { get; set; }

    [ForeignKey(nameof(UsersTasksId))]
    public UsersTasks? UsersTasks { get; set; }
}
