using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using MadeHuman_Server.Model.Shop;
using MadeHuman_Server.Model.WareHouse;
using System.Text.Json.Serialization;

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
        public Guid Id { get; set; } = default!;

        public int Quantity { get; set; } = default!;

        public StatusProductBatches StatusProductBatches { get; set; }

        // Sửa lại chỗ này
        public Guid ProductSKUId { get; set; } = default!;


        [ForeignKey("ProductSKUId")]
        [JsonIgnore]
        public ProductSKU ProductSKUs { get; set; }

        public Guid InboundTaskId { get; set; } = default!;

        [ForeignKey("InboundTaskId")]
        public InboundTasks InboundTasks { get; set; }

        public Guid WarehouseLocationId { get; set; } = default!;

        [ForeignKey("WarehouseLocationId")]
        public WarehouseLocations WarehouseLocation { get; set; }
    }

}