using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_Share.ViewModel
{
    public class CheckInCheckOutTodayViewModel
    {
        public string UserId { get; set; } = default!;
        public string PartTimeId { get; set; } = default!;

        public DateTime Timestamp { get; set; }
        public bool IsCheckIn { get; set; }
        public bool IsOvertime { get; set; }
        public string? Note { get; set; }
    }
}
