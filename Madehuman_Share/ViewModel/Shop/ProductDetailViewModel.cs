using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_Share.ViewModel.Shop
{
    public class ProductDetailViewModel
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public decimal Price { get; set; }

        public string? SKU { get; set; } // Lấy từ Product.ProductSKU.SKU

        public int QuantityInStock { get; set; }
        public string? CategoryName { get; set; }

        public List<ProductItemDto> ProductItems { get; set; } = new();
    }


    public class ProductItemDto
    {
        public string SKU { get; set; } = "";
        public int QuantityInStock { get; set; }
    }

}
