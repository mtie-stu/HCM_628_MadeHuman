using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_User.ViewModel.WareHouse
{
    public class WareHouseViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime LastUpdated{ get; set; }
    }
}
