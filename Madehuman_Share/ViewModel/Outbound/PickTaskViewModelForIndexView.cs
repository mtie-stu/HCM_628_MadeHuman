using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_Share.ViewModel.Outbound
{
    public class PickTaskViewModelForIndexView
    {
        public Guid Id { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? FinishAt { get; set; }
        public string Status { get; set; }

        public Guid? OutboundTaskId { get; set; }
        public string? OutboundTaskCode { get; set; }

        public Guid? OutboundTaskItemId { get; set; }
        public string? ProductSKUCode { get; set; }
        public int TotalQuantity { get; set; } // 🔢 Tổng số lượng PickTaskDetails

    }
}
