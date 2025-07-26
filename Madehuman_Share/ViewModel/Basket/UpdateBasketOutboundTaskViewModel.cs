using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_Share.ViewModel.Basket
{
    public class UpdateBasketOutboundTaskViewModel
    {
        public Guid BasketId { get; set; }
        public StatusBasketsvm? Status { get; set; } // Optional: cho phép cập nhật nếu muốn

        public Guid? OutBoundTaskId { get; set; } // null = remove
    }

}
