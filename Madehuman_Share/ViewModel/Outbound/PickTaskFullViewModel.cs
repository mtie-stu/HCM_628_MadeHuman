using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_Share.ViewModel.Outbound
{
    public enum StatusPickTaskvm
    {
        Created,
        Recived,
        Finished
    }
    public class PickTaskFullViewModel
    {
        public Guid Id { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime FinishAt { get; set; }
        public StatusPickTaskvm Status { get; set; }
        public Guid? UsersTasksId { get; set; }

        public List<PickTaskDetailItem> Details { get; set; }

        public class PickTaskDetailItem
        {
            public Guid Id { get; set; }
            public int Quantity { get; set; }
            public Guid WarehouseLocationId { get; set; }
            public Guid ProductSKUId { get; set; }
        }
    }

}
