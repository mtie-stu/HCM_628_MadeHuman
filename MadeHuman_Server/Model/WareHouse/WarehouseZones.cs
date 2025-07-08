using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace MadeHuman_Server.Model.WareHouse
{
    public class WarehouseZones
    {
        [Key]
        public Guid Id { get; set; } = default!;       
        public string Name { get; set; } = string.Empty;    
        public Guid WarehouseId { get; set; } = default!;
        [ForeignKey(nameof(WarehouseId))]
        public WareHouse WareHouse { get; set; }
        public ICollection<WarehouseLocations> WarehouseLocations { get; set; }
    }
}