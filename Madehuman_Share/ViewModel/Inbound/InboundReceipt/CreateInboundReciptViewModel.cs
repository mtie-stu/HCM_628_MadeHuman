using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_User.ViewModel.Inbound.InboundReceipt
{
    public class CreateInboundReceiptViewModel
    {
        public Guid InboundReceiptId { get; set; } = Guid.NewGuid();
        public DateTime ReceivedAt { get; set; } = DateTime.UtcNow;
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;


        public List<InboundReceiptItemViewModel> Items { get; set; } = new();
    }

    public class InboundReceiptItemViewModel
    {
        public Guid InboundReceiptItemId { get; set; } = Guid.NewGuid();
        public Guid ProductSKUId { get; set; }
        public int Quantity { get; set; }
    }
}
