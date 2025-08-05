using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_Share.ViewModel.Inbound
{
    public class RefillTaskDetailWithHeaderViewModel
    {
        public Guid RefillTaskId { get; set; }
        public string CreateBy { get; set; }
        public DateTime CreateAt { get; set; }
        public Guid? LowStockId { get; set; }
        public Guid? UserTaskId { get; set; }
        public Guid? ProductSKUId { get; set; }
        public string SKU { get; set; }
        public Guid DetailId { get; set; }
        public Guid FromLocation { get; set; }
        public Guid ToLocation { get; set; }
        public string? FromLocationName { get; set; }  // <- Gán từ service
        public string? ToLocationName { get; set; }

        public int Quantity { get; set; }
        public bool IsRefilled { get; set; } // ✅ Thêm dòng này
    }
}
