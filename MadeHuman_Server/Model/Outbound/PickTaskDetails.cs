using MadeHuman_Server.Model.Shop;
using MadeHuman_Server.Model.WareHouse;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MadeHuman_Server.Model.Outbound
{
    public class PickTaskDetails
    {
        public Guid Id { get; set; }
        public int Quantity { get; set; }
        public Guid WarehouseLocationId { get; set; }
        [ForeignKey("WarehouseLocationId")]
        public WarehouseLocations WarehouseLocation { get; set; }
        public bool IsPicked { get; set; }  = false;
        public Guid ProductSKUId { get; set; } = default!;
        public int QuantityPicked { get; set; } = 0;


        [ForeignKey("ProductSKUId")]
        [JsonIgnore]
        public ProductSKU ProductSKUs { get; set; }
        public Guid PickTaskId { get; set; } // KHÔNG phải PicksTasksId

        [ForeignKey(nameof(PickTaskId))]
        public PickTasks PickTask { get; set; }

    }
}
