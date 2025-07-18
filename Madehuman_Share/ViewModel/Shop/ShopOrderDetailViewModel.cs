using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_User.ViewModel.Shop
{

    public class ShopOrderDetailViewModel
    {
        public Guid ShopOrderId { get; set; }
        public string AppUserId { get; set; }
        public string? AppUserEmail { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public StatusOrder Status { get; set; }

        public List<OrderItemViewModel> Items { get; set; } = new();
    }

    public class OrderItemViewModel
    {
        public Guid ProductSKUsId { get; set; }
        public string? ProductSKUCode { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

}
