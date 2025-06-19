using System.ComponentModel.DataAnnotations;

namespace MadeHuman_Server.Model.Shop
{
    public class Category
    {
        [Key]
        public Guid CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        // Navigation property
        public ICollection<Product> Products { get; set; }
    }
}
