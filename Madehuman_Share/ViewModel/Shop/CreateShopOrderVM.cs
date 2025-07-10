using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_Share.ViewModel.Shop
{
    public enum StatusOrder
    {
        Created,
        Confirmed,
        Processing,
        Delivery
    }
    public class CreateShopOrderWithMultipleItems
    {
        [Required]
        public string AppUserId { get; set; }

        [Required]
        public StatusOrder Status { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public List<OrderItemInputModel> Items { get; set; } = new();
    }

    public class OrderItemInputModel
    {
        /*public string ProductSKUs { get; set; } = null!;*/ // ✅ đảm bảo không null // sửa lại tên từ Producttd
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }
        public Guid ProductSKUsId { get; set; }
    }
}
