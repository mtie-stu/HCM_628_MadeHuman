using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MadeHuman_Server.Model.Shop
{
    public class Combo
    {
        [Key]
        public Guid ComboId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Discount { get; set; }

        // Navigation property
        public ICollection<ProductSKU> ProductSKUs { get; set; } // Thêm quan hệ mới

        public ICollection<ComboItem> ComboItems { get; set; }
    }

}
