using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_User.ViewModel.Shop
{
    public class ComboListItemViewModel
    {
        public Guid ComboId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        // Optional: hiển thị SKU nếu có
        public string SKU { get; set; }

        // Optional: hiển thị tổng số sản phẩm trong combo
        public List<ComboItemDto> ComboItems { get; set; } = new();
    }
    public class ComboDetailViewModel
    {
        public Guid ComboId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string? SKU { get; set; }
        public List<ComboItemDto> ComboItems { get; set; } = new();

    }
    public class ComboItemDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
    }
}
