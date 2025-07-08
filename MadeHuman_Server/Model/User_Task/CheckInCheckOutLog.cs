using MadeHuman_Server.Model.User_Task;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class CheckInCheckOutLog
{
    [Key]
    public Guid Id { get; set; } = default!;

    [Required]
    public string PartTimeId { get; set; } = string.Empty;

    [Required]
    public string UserId { get; set; } = string.Empty;

    [ForeignKey(nameof(UserId))]
    public AppUser User { get; set; }

    public DateTime Timestamp { get; set; } = default!;

    public bool IsCheckIn { get; set; } = default!; // true = checkin, false = checkout

    public bool IsOvertime { get; set; } = default!; // ✅ Mới: true nếu là log tăng ca

    public string? Note { get; set; }

    public Guid? UsersTasksId { get; set; }

    [ForeignKey(nameof(UsersTasksId))]
    public UsersTasks? UsersTasks { get; set; }
}
