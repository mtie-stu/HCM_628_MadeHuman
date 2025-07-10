using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_Share.ViewModel.Shop
{
    public class SKUInfoViewModel
    {
        public string SKU { get; set; }

        public string Type { get; set; } // "Product" hoặc "Combo"

        // Nếu là Product
        public Guid? ProductId { get; set; }
        public string? ProductName { get; set; }
        public decimal? ProductPrice { get; set; }

        // Nếu là Combo
        public Guid? ComboId { get; set; }
        public string? ComboName { get; set; }
        public decimal? ComboPrice { get; set; }
        public List<ComboItemViewModel>? ComboItems { get; set; }
    }

    public class ComboItemViewModel
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
    }

}
