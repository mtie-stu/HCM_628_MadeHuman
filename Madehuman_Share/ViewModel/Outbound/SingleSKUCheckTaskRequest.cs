using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_Share.ViewModel.Outbound
{
    public class SingleSKUCheckTaskRequest
    {
        public Guid CheckTaskId { get; set; }
        public string SKU { get; set; } = null!;
        public int Quantity { get; set; }
    }

}
