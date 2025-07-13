using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_Share.ViewModel.Inbound.InboundReceipt
{
    public class InboundReceiptDetailViewModel
    {
        public Guid Id { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime ReceivedAt { get; set; }
        public string Status { get; set; } = string.Empty;

        public string? TaskCode { get; set; }
        public string? TaskStatus { get; set; }

        public List<InboundReceiptItemDetail> Items { get; set; } = new();
    }

    public class InboundReceiptItemDetail
    {
        public Guid ProductSKUId { get; set; }
        public string? ProductSKUName { get; set; } // cần lấy tên SKU từ ProductSKU
        public int Quantity { get; set; }
    }

}
