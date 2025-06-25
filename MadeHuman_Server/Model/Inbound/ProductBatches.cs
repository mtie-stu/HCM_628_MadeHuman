using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using MadeHuman_Server.Model.Shop;
using MadeHuman_Server.Model.WareHouse;

namespace MadeHuman_Server.Model.Inbound
{
    public enum StatusProductBatches
    {
        UnStored,
        Stored


    }
    public class ProductBatches
    {

        [Key]
        public Guid Id { get; set; }
        public int Quantity { get; set; }

        public StatusProductBatches StatusProductBatches { get; set; }
        [ForeignKey(nameof(ProductSKUs))]
        public Guid ProductSKUId { get; set; }
        public string ProductSKU { get; set; }

        public ProductSKU ProductSKUs{ get; set; }

        [ForeignKey(nameof(InboundTasks))]
        public Guid InboundTaskId { get; set; }
        // Thêm liên kết 1-1 với WarehouseLocation
        public Guid WarehouseLocationId { get; set; }
        [ForeignKey(nameof(WarehouseLocationId))]
        public WarehouseLocations WarehouseLocation { get; set; }

        public InboundTasks InboundTasks { get; set; }

    }
}