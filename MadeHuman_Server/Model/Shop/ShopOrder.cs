using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MadeHuman_Server.Model.User_Task;

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
        public Guid ShopOrderId { get; set; } = default!;

        [Required]
        public DateTime OrderDate { get; set; } = default!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; } = default!;

       
        public StatusOrder Status { get; set; }
        // Thêm khóa ngoại đến AppUser
        public string AppUserId { get; set; } = string.Empty; // Kiểu string vì IdentityUser.Id là string

        // Navigation property
        [ForeignKey("AppUserId")]
        public AppUser AppUser { get; set; }
        [JsonIgnore]
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
