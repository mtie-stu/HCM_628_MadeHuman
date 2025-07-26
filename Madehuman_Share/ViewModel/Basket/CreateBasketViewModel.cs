using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_Share.ViewModel.Basket
{
    public enum StatusBasketsvm
    {
        Empty,
        Selected
    }
    public class CreateBasketViewModel
    {
        public StatusBasketsvm Status { get; set; } = StatusBasketsvm.Empty;
        public Guid? OutBoundTaskId { get; set; }
    }

}
