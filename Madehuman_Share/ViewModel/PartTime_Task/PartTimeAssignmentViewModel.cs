using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_User.ViewModel.PartTime_Task
{
    public enum TaskTypevm
    {
        Picker,
        Checker,
        Packer,
        Dispatcher
    }
    public class PartTimeAssignmentViewModel
    {
        public Guid? Id { get; set; } // null nếu thêm mới
        public Guid? PartTimeId { get; set; }
        public DateOnly WorkDate { get; set; }
        public TaskTypevm TaskType { get; set; }
        public string? ShiftCode { get; set; }
        public bool IsConfirmed { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public TimeSpan? BreakDuration { get; set; }
        public string? Note { get; set; }
        public string? UserId { get; set; }
        public Guid? UsersTasksId { get; set; }
        public Guid? CompanyId { get; set; }
    }

}
