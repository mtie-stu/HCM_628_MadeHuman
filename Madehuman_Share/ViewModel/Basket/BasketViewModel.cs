using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_Share.ViewModel.Basket
{
    public class BasketViewModel   //BasketViewModel (để hiển thị)
    {
        public Guid Id { get; set; }
        public string Status { get; set; } // hiển thị dạng chữ (Empty / Selected)
        public Guid? OutBoundTaskId { get; set; }
    }

}
