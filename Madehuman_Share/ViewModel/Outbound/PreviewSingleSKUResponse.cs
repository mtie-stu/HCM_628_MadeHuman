using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_Share.ViewModel.Outbound
{
    public class PreviewSingleSKUResponse
    {
        public string SKU { get; set; }
        public string ProductName { get; set; }
        public int RequiredQuantity { get; set; }
        public List<string> ImageUrls { get; set; }

    }
}
