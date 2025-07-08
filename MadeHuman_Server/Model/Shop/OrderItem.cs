using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MadeHuman_Server.Model.Shop
{
    public class OrderItem
    {
        [Key]
        public Guid OrderItemId { get; set; } = default!;

        public int Quantity { get; set; } = default!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; } = default!;

        // Foreign keys
        public Guid ProductSKUsId { get; set; } = default!;       // Nullable vì có thể thuộc ProductItem

        public Guid ShopOrderId { get; set; } = default!;
        public string ProductSKU { get; set; } = string.Empty;
        //public Guid? ProductSKUId { get; set; }


        // Navigation properties
        [JsonIgnore]
        public ShopOrder ShopOrder { get; set; }
        [ForeignKey("ProductSKUsId")]
        public ProductSKU ProductSKUs { get; set; }
       

    }
}
