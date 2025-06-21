using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MadeHuman_Server.Model.Shop
{                                               
    public enum StatusOrder
    {
        Created,
        Confirmed,
        Processing,
        Delivery,
        Cancel
    }
    public class ShopOrder
    {
        [Key]
        public Guid ShopOrderId { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

       
        public StatusOrder Status { get; set; }
        // Thêm khóa ngoại đến AppUser
        public string AppUserId { get; set; } // Kiểu string vì IdentityUser.Id là string

        // Navigation property
        [ForeignKey("AppUserId")]
        public AppUser AppUser { get; set; }
        [JsonIgnore]
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
