using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_Share.ViewModel.Outbound
{
    public class OutboundTaskItemPreviewViewModel
    {
        public Guid OutboundTaskItemId { get; set; }
        public Guid OutboundTaskId { get; set; }
        public Guid ShopOrderId { get; set; }
        public string Status { get; set; } = "";
        public List<OutboundTaskItemDetailViewModel> Details { get; set; } = new();
    }
    public class OutboundTaskItemDetailViewModel
    {
        public Guid OutboundTaskItemDetailId { get; set; }
        public Guid ProductSKUId { get; set; }
        public string? SKU { get; set; }
        public int Quantity { get; set; }
    }

}
