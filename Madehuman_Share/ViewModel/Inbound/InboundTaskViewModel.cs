using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_Share.ViewModel.Inbound
{
    public class InboundTaskViewModel
    {
        public Guid Id { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateAt { get; set; }
        public string Status { get; set; }
        public Guid InboundReceiptId { get; set; }
        public Guid? UserTaskId { get; set; }
    }
}
