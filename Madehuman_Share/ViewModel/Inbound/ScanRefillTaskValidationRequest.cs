using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_Share.ViewModel.Inbound
{
    public class ScanRefillTaskValidationRequest
    {
        public Guid RefillTaskId { get; set; }
        public Guid RefillTaskDetailId { get; set; }
        public string? FromLocationName { get; set; }
        public string? ToLocationName { get; set; }
        public string? SKU { get; set; }
        public int? Quantity { get; set; }
    }
}
