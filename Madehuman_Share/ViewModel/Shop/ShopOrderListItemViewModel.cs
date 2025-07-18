using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_User.ViewModel.Shop
{
    public class ShopOrderListItemViewModel
    {
        public Guid ShopOrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public string AppUserName { get; set; }
        public int ItemCount { get; set; }
    }
}
