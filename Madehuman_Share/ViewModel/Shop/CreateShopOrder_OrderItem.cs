using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_User.ViewModel.Shop
{
    public class CreateShopOrder_OrderItem
    {
        public Guid ShopOrderId { get; set; }

        public DateTime OrderDate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

    
        public string Status { get; set; }
        // Thêm khóa ngoại đến AppUser
        public string AppUserId { get; set; } // Kiểu string vì IdentityUser.Id là string

        public Guid OrderItemId { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        // Foreign keys
        public Guid ProductId { get; set; }
    }
}
