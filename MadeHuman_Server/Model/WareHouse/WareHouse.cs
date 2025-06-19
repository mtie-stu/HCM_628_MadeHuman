using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace MadeHuman_Server.Model.WareHouse
{
    public class WareHouse
    {
        [Key]
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Location { get; set; }
        public DateTime LastUpdated { get; set; }

        public ICollection<WarehouseZones> WarehouseZones { get; set; }
        public Guid WarehouseId { get; set; }

    }
}