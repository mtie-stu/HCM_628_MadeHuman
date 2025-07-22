using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 

namespace Madehuman_Share.ViewModel.Outbound
{
    public class CheckTaskFullViewModel
    {
        public Guid Id { get; set; }
        public DateTime CreateAt { get; set; }
        public int Status { get; set; }
        public Guid? UsersTasksId { get; set; }
        public Guid OutboundTaskId { get; set; }

        public List<CheckTaskDetailItem> Details { get; set; } = new();

        public class CheckTaskDetailItem
        {
            public Guid Id { get; set; }
            public DateTime CreateAt { get; set; }
            public int Status { get; set; }
            public OutboundTaskItemVm OutboundTaskItem { get; set; } = new();
        }
    }

    public class OutboundTaskItemVm
    {
        public Guid Id { get; set; }
        public string? Note { get; set; }
        public List<OutboundTaskItemDetailVm> ItemDetails { get; set; } = new();
    }

    public class OutboundTaskItemDetailVm
    {
        public Guid Id { get; set; }
        public Guid ProductSKUId { get; set; }
        public int Quantity { get; set; }
    }



}
