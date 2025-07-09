using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_Share.ViewModel.Inbound
{
   public  class GetInboundTaskById_Viewmodel
    {
        public Guid InboundTaskId { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateAt { get; set; }
        public string Status { get; set; }

        public List<BatchInfoViewModel> ProductBatches { get; set; }
    }
    public class BatchInfoViewModel
    {
        public Guid ProductBatchId { get; set; }
        public int Quantity { get; set; }
        public string StatusProductBatch { get; set; }
        public string SKU { get; set; }
        public string NameLocation { get; set; }
    }
}
