using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_User.ViewModel.Shop
{
    public class ProductListItemViewModel
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = "";
        public decimal Price { get; set; }
        public string? SKU { get; set; }
        public string? CategoryName { get; set; }
    }
}
