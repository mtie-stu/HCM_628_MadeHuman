using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MadeHuman_Server.Model.Shop
{
    public class Product
    {
        [Key]
        public Guid ProductId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        // Foreign key
        public int CategoryId { get; set; }

        // Navigation properties
        public Category Category { get; set; }
        public ICollection<ProductItem> ProductItems { get; set; }
        public ICollection<ProductSKU> ProductSKUs { get; set; } // Thêm quan hệ mới

        public ICollection<ComboItem> ComboItems { get; set; }
    }

}
