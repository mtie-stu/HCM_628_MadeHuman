using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_Share.ViewModel.WareHouse
{
   public class WareHouseZoneViewModel
   {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid WarehouseId { get; set; }
   }
}
