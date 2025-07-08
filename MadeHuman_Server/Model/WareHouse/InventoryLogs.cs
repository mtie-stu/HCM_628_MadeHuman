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
        public Guid Id { get; set; }

        public int StockQuantity { get; set; }
        public int ChangeQuantity { get; set; }

        public string ChangeBy{ get; set; }

        public ActionInventoryLogs ActionInventoryLogs {  get; set; }
        public int RemainingQuantity { get; set; }

        public DateTime Time {  get; set; } 
        public Guid InventoryId { get; set; }

        [ForeignKey(nameof(InventoryId))]
        public Inventory Inventory { get; set; }
    }
}