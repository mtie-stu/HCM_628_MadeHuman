using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MadeHuman_Server.Model.Shop
{
    public class ShopOrder
    {
        [Key]
        public Guid ShopOrderId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; }
        // Thêm khóa ngoại đến AppUser
        public string AppUserId { get; set; } // Kiểu string vì IdentityUser.Id là string

        // Navigation property
        public AppUser AppUser { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
