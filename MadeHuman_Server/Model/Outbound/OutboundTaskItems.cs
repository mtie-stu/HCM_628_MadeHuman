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
        public OutboundTask OutboundTask   { get; set; }    
        public OutboundTaskItemDetails OutboundTaskItemDetails { get; set; }


    }
}
