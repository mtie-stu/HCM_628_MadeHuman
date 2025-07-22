using Google.Apis.Drive.v3.Data;
using MadeHuman_Server.Model.Shop;
using System.ComponentModel.DataAnnotations.Schema;

namespace MadeHuman_Server.Model.Outbound
{
    public enum StatusOutboundTaskItems
    {
        Created,
        Picked,
        Checked,
        Dispatched,
        Finished
    }
    public class OutboundTaskItems
    {
        public Guid Id { get; set; }
       public StatusOutboundTaskItems Status { get; set; }  
       public Guid ShopOrderId { get; set; }
        [ForeignKey(nameof(ShopOrderId))]
        public ShopOrder ShopOrder { get; set; }

        [ForeignKey(name: "OutboundTask")]
        public Guid OutboundTaskId { get; set; }
        public OutboundTask OutboundTask   { get; set; }    
        public OutboundTaskItemDetails OutboundTaskItemDetails { get; set; }

        public CheckTaskDetails CheckTaskDetails { get; set; }  // 1-1

    }
}
