using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using MadeHuman_Server.Model.WareHouse;
using MadeHuman_Server.Model.Shop;

namespace MadeHuman_Server.Model.Inbound
{
    public class Inventory
    {
        [Key]
        public Guid Id { get; set; } = default!;

        public  int?  StockQuantity { get; set; }
        public int? QuantityBooked { get; set; }
        public DateTime LastUpdated{  get; set; } = default!;   

        public Guid? ProductSKUId { get; set; }
        [ForeignKey(nameof(ProductSKUId))]
        public ProductSKU ProductSKUs { get; set; }


        public Guid WarehouseLocationId { get; set; } = default!;
        [ForeignKey(nameof(WarehouseLocationId))]
        public WarehouseLocations WarehouseLocations { get; set; }

        public ICollection<InventoryLogs> InventoryLogs { get; set; }

    }
}                                        