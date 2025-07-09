using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace MadeHuman_Server.Model.WareHouse
{
    public class WareHouse
    {
        [Key]
        public Guid Id { get; set; } = default!;

        public string Name { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; } = default!;

        public ICollection<WarehouseZones> WarehouseZones { get; set; }
        public Guid WarehouseId { get; set; } = default!;

    }
}