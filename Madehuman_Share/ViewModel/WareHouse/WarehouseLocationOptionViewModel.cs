using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Madehuman_Share.ViewModel.WareHouse
{
    public class WarehouseLocationOptionViewModel
    {
        public Guid Id { get; set; }
        public string NameLocation { get; set; } = default!;
        public Guid ZoneId { get; set; }
        public string StatusWareHouse { get; set; }
    }
}
