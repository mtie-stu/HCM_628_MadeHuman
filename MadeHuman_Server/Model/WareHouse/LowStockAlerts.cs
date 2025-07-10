using MadeHuman_Server.Model.Inbound;
using System.ComponentModel.DataAnnotations.Schema;

namespace MadeHuman_Server.Model.WareHouse
{
    public enum StatusLowStockAlerts
    {
        Empty,
        Normal,
        Warning
    }
    public class LowStockAlerts
    {
        public Guid Id { get; set; }
        public int CurrentQuantity { get; set; }
        public StatusLowStockAlerts StatusLowStockAlerts {  get; set; } 

        public Guid WarehouseLocationId { get; set; }
        [ForeignKey("WarehouseLocationId")]
        public WarehouseLocations WarehouseLocations { get; set; }
        public RefillTasks RefillTasks { get; set; } // điều hướng 1-1

    }
}
