using MadeHuman_Server.Model.Shop;
using Microsoft.AspNetCore.Identity;

namespace MadeHuman_Server.Model
{
    public class AppUser : IdentityUser
    {
        public enum UserType
        {
            Warehouse,
            Customer
        }
        public enum UserStatus
        {
            InActive,
            Active,
            Banned
          
        }

        public string? Name { get; set; }
        public string? Image { get; set; }
        public ICollection<ShopOrder> ShopOrders { get; set; }
        public  UserType UserTypes { get; set; }
        public UserStatus Status { get; set; }


    }
}
