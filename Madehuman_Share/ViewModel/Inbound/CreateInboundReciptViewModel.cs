using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_Share.ViewModel.Inbound
{
    public class CreateInboundReceiptViewModel
    {
        public Guid InboundReceiptId { get; set; } = Guid.NewGuid();
        public DateTime ReceivedAt { get; set; }
        public DateTime CreateAt{ get; set; }

        public List<InboundReceiptItemViewModel> Items { get; set; } = new();
    }

    public class InboundReceiptItemViewModel
    {
        public Guid InboundReceiptItemId { get; set; } = Guid.NewGuid();
        public Guid ProductSKUId { get; set; }
        public int Quantity { get; set; }
    }
}
