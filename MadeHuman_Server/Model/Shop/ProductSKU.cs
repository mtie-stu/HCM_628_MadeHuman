using MadeHuman_Server.Model.Inbound;
using System.ComponentModel.DataAnnotations;

namespace MadeHuman_Server.Model.Shop
{
    public class ProductSKU
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string SKU { get; set; }

        // Foreign keys
        public Guid? ProductItemId { get; set; } // Nullable vì có thể thuộc Combo
        public Guid? ComboId { get; set; }       // Nullable vì có thể thuộc ProductItem

        // Navigation properties
        public Product Product { get; set; }
        public Combo Combo { get; set; }
        public ICollection<Inventory> Inventory { get; set; }

    }
}
