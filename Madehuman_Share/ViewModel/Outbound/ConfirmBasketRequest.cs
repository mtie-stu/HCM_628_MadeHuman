using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_Share.ViewModel.Outbound
{
    public class ConfirmBasketRequest
    {
        public Guid? PickTaskId { get; set; }
        public Guid BasketId { get; set; }
    }
}
