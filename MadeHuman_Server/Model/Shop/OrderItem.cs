using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MadeHuman_Server.Model.Shop
{
    public class OrderItem
    {
        [Key]
        public Guid OrderItemId { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        // Foreign keys
        public Guid ShopOrderId { get; set; }
        public Guid ProductItemId { get; set; }

        // Navigation properties
        public ShopOrder ShopOrder { get; set; }
        public ProductItem ProductItem { get; set; }
    }
}
