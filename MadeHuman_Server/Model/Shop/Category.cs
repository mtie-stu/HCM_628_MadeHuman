using System.ComponentModel.DataAnnotations;

namespace MadeHuman_Server.Model.Shop
{
    public class Category
    {
        [Key]
        public Guid CategoryId { get; set; } = default!;

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        // Navigation property
        public ICollection<Product> Products { get; set; }
    }
}
