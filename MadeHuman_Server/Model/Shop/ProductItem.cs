using System.ComponentModel.DataAnnotations;

namespace MadeHuman_Server.Model.Shop
{
    public class ProductItem
    {
        [Key]
        public Guid ProductItemId { get; set; }

        [Required]
        [StringLength(50)]
        public string SKU { get; set; }

        public int QuantityInStock { get; set; }

        // Foreign key
        public Guid ProductId { get; set; }

        // Navigation properties
        public Product Product { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
