using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_User.ViewModel.Inbound.InboundReceipt
{
    public class InboundReceiptViewModel
    {
        public Guid Id { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime ReceivedAt { get; set; }
        public string Status { get; set; }

        public string? TaskCode { get; set; } // nếu có
        public string? TaskStatus { get; set; } // InboundTasks.Status => string
        public int ItemCount { get; set; }
    }
}
