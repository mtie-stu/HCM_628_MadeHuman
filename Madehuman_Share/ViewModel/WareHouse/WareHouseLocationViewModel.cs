using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_Share.ViewModel.WareHouse
{
    public class WarehouseLocationViewModel
    {
        public Guid Id { get; set; }
        public string NameLocation { get; set; }
        public Guid ZoneId { get; set; }

    }
}
