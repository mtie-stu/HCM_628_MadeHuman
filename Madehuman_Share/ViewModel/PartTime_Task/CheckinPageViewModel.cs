using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_Share.ViewModel.PartTime_Task
{
    public class CheckinPageViewModel
    {
        public Checkin_Checkout_Viewmodel Form { get; set; } = new();
        public List<CheckInCheckOutTodayViewModel> TodayLogs { get; set; } = new();
    }
}
