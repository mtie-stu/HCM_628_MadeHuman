using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_Share.ViewModel.Inbound
{
    public class ScanInboundTaskValidationRequest
    {
        public Guid InboundTaskId { get; set; }
        public Guid ProductBatchId { get; set; }  // ✅ Bổ sung để xác định chính xác batch
        public string? NameLocation { get; set; }
        public string? SKU { get; set; }
        public int? Quantity { get; set; }
    }
}
