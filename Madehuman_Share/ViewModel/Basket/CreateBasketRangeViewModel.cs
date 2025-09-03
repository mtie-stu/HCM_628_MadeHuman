using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_Share.ViewModel.Basket
{
    public class CreateBasketRangeViewModel
    {
        public int Quantity { get; set; }
        public StatusBasketsvm Status { get; set; } = StatusBasketsvm.Empty;
    }

}
