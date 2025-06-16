 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_Share.ViewModel.WareHouse
{
    public class GenerateWHLocationRequest
    {
        public Guid ZoneId { get; set; }
        public int Quantity { get; set; }


        public char StartLetter { get; set; }
        public char EndLetter { get; set; }

        public int StartNumber { get; set; }
        public int EndNumber { get; set; }

        public int StartSub { get; set; }
        public int EndSub { get; set; }

    }
}
