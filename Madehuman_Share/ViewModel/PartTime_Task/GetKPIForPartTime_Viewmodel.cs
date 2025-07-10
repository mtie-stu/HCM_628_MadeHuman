using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_Share.ViewModel.PartTime_Task
{
    public enum TaskTypeUservm
    {
        Picker,
        Checker,
        Packer,
        Dispatcher
    }
    public class GetKPIForPartTime_Viewmodel
    {

        public TaskTypeUservm TaskType { get; set; }
        public DateTime WorkDate { get; set; }
        public string Email { get; set; }
        public Guid? PartTimeId { get; set; }
        public int TotalKPI { get; set; }
        public int HourlyKPIs { get; set; }
    }
}
