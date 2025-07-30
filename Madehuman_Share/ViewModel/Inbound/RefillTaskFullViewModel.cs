using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_Share.ViewModel.Inbound
{
    public class RefillTaskFullViewModel
    {
        // Thông tin Refill Task
        public Guid Id { get; set; }
        public Guid? LowStockId { get; set; }
        public Guid? UserTaskId { get; set; }
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
        public string CreateBy { get; set; }
        public string? CreateByName { get; set; }


        // Danh sách chi tiết Refill
        public List<RefillTaskDetailItem> Details { get; set; } = new();

        public class RefillTaskDetailItem
        {
            public Guid Id { get; set; }    
            public Guid? ProductSKUId { get; set; }     // có thể null nếu nhập theo SKU
            public string? SKU { get; set; }            // SKU dạng text
            public Guid FromLocation { get; set; }
            public Guid ToLocation { get; set; }
            public int Quantity { get; set; }   
        }
    }

}
