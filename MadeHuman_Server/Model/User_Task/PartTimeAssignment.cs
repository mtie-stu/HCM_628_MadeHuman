using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace MadeHuman_Server.Model.User_Task
{
    public enum TaskType
    {
        Picker,
        Checker,
        Packer,
        Dispatcher
    }

    public class PartTimeAssignment
    {
        [Key]
        public Guid Id { get; set; }

        public Guid? PartTimeId { get; set; }         // Mã nhân sự part-time (mã từ công ty nhân lực)
        [ForeignKey(nameof(PartTimeId))]
        public PartTime PartTime { get; set; }
        public DateOnly WorkDate { get; set; }         // Ngày phân công

        public TaskType TaskType { get; set; }         // Loại nhiệm vụ (enum chuẩn hóa)

        public string? ShiftCode { get; set; }         // Mã ca: sáng/chiều/tối

        public bool IsConfirmed { get; set; } = false; // Đã xác nhận phân công từ đối tác
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }

        public TimeSpan? BreakDuration { get; set; }
        public string? Note { get; set; }              // Ghi chú

        // FK tới AppUser – tài khoản được phân công
        public string? UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public AppUser? User { get; set; }

        // FK tới UsersTasks – nếu đã có ca làm thực tế tương ứng
        public Guid? UsersTasksId { get; set; }

        [ForeignKey(nameof(UsersTasksId))]
        public UsersTasks? UsersTasks { get; set; }
        // ✅ Thêm FK đến Company
        public Guid? CompanyId { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public Part_Time_Company part_Time_Company { get; set; }
    }
}
        