using MadeHuman_Server.Model.Inbound;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MadeHuman_Server.Model.Shop
{
    public class ProductSKU
    {
        [Key]
        public Guid Id { get; set; }    

        [Required]
        [StringLength(50)]
        public string? SKU { get; set; }

        // Foreign keys
        public Guid? ProductId { get; set; } // Nullable vì có thể thuộc Combo
        public Guid? ComboId { get; set; }       // Nullable vì có thể thuộc ProductItem

        // Navigation properties
        [JsonIgnore]
        public Product Product { get; set; }
        [JsonIgnore]
        public Combo Combo { get; set; }
        public ICollection<Inventory> Inventory { get; set; }
        public int? QuantityInStock { get; set; } = default!;


    }
}
