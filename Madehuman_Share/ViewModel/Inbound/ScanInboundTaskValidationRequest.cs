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
    public class ValidateScanSuccessResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public class ValidateScanErrorResponse
    {
        public bool Success { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
