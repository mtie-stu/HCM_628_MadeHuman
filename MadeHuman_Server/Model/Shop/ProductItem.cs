using System.ComponentModel.DataAnnotations;

namespace MadeHuman_Server.Model.Shop
{
    public class ProductItem
    {
        [Key]
        public Guid ProductItemId { get; set; } = default!;

        [Required]
        [StringLength(50)]
        public string SKU { get; set; } = string.Empty;

        //public int QuantityInStock { get; set; } = default!;

        // Foreign key
        public Guid ProductId { get; set; } = default!;

        // Navigation properties
        public Product Product { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
