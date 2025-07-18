using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_User.ViewModel.PartTime_Task
{
    public enum StatusPartTimevm
    {
        PartTime,
        PTCD,
        Banned
    }
    public class PartTimeViewModel
    {
        public Guid? PartTimeId { get; set; } // null nếu thêm
        public string Name { get; set; }
        public string CCCD { get; set; }
        public string PhoneNumber { get; set; }
        public StatusPartTimevm StatusPartTimes { get; set; }
        public Guid CompanyId { get; set; }
    }
}
