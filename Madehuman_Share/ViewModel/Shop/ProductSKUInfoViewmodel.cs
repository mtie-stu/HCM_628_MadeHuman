using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_Share.ViewModel.Shop
{
    public class ProductSKUInfoViewmodel
    {
        public Guid ProductSKUId { get; set; }
        public string ProductName { get; set; }
        public string SkuCode { get; set; }
        public List<string> ImageUrls { get; set; } // thay vì string ImageUrl
    }
}
