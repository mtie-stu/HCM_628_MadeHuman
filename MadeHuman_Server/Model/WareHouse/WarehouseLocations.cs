using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using MadeHuman_Server.Model.WareHouse;
using MadeHuman_Server.Model.Inbound;
using System.Text.Json.Serialization;

namespace MadeHuman_Server.Model.WareHouse
{
    public enum StatusWareHouse
    {
        Empty,
        Booked,
        Stored

    }
    public class WarehouseLocations
    {
        [Key]
        public Guid Id { get; set; } = default!;
        public string NameLocation { get; set; } = string.Empty;
        public StatusWareHouse StatusWareHouse { get; set; }    
        public Guid ZoneId { get; set; } = default!;
        [ForeignKey(nameof(ZoneId))]
        public WarehouseZones WarehouseZones { get; set; }
        public Inventory Inventory { get; set; }
        [JsonIgnore] // Ngăn vòng lặp khi serialize
        public ProductBatches ProductBatch { get; set; }
        public Guid? LowStockId { get; set; } = default!;

        [ForeignKey("LowStockId")]
        public LowStockAlerts LowStockAlerts { get; set; }

    }
}