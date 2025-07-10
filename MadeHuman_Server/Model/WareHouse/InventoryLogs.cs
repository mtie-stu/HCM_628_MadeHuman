using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using MadeHuman_Server.Model.Shop;
namespace MadeHuman_Server.Model.Inbound
{
    public enum ActionInventoryLogs
    {
        Refill,
        Take,
        
    }
    public class InventoryLogs
    {

        [Key]
        public Guid Id { get; set; } = default!;

        public int StockQuantity { get; set; } = default!;
        public int ChangeQuantity { get; set; } = default!;

        public string ChangeBy{ get; set; } = string.Empty;

        public ActionInventoryLogs ActionInventoryLogs {  get; set; }
        public int RemainingQuantity { get; set; } = default!;

        public DateTime Time {  get; set; } = default!; 
        public Guid InventoryId { get; set; } = default!;

        [ForeignKey(nameof(InventoryId))]
        public Inventory Inventory { get; set; }
    }
}