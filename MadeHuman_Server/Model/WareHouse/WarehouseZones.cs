using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace MadeHuman_Server.Model.WareHouse
{
    public class WarehouseZones
    {
        [Key]
        public Guid Id { get; set; }       
        public string Name { get; set; }    
        public Guid WarehouseId { get; set; }
        [ForeignKey(nameof(WarehouseId))]
        public WareHouse WareHouse { get; set; }
        public ICollection<WarehouseLocations> WarehouseLocations { get; set; }
    }
}