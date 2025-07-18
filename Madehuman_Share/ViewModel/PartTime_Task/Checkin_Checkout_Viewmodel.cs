using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Madehuman_User.ViewModel.PartTime_Task
{
    public enum Regime
    {
        Checkin,
        Checkout,
        Break
        
    }
    public class Checkin_Checkout_Viewmodel
    {
        public string UserId { get; set; }
        public Regime regime { get; set; }  
        public Guid? PartTimeId { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public DateTime WorkDate { get; set; }
        public TimeSpan? BreakDuration { get; set; }
        public TimeSpan? OvertimeDuration { get; set; }
        public bool IsCompleted { get; set; } = false;
        public string? Note { get; set; }


    }
}
