using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_Share.ViewModel.Shop
{
    public class GenerateRandomOrdersSingleRequest
    {
        public string ProductSKU { get; set; }
        public decimal UnitPrice { get; set; }
        public Guid AppUserId { get; set; }
        public StatusOrder Status { get; set; }
        public int NumberOfOrders { get; set; }
        public int MinQuantity { get; set; }
        public int MaxQuantity { get; set; }
        public DateTime? BaseOrderDate { get; set; }
    }
}
