using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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
        public Guid ProductSKUsId { get; set; }       // Nullable vì có thể thuộc ProductItem

        public Guid ShopOrderId { get; set; }
        public string ProductSKU { get; set; }
        //public Guid? ProductSKUId { get; set; }


        // Navigation properties
        [JsonIgnore]
        public ShopOrder ShopOrder { get; set; }
        [ForeignKey("ProductSKUsId")]
        public ProductSKU ProductSKUs { get; set; }
       

    }
}
