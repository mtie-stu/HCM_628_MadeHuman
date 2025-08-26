using Google.Apis.Drive.v3.Data;
using MadeHuman_Server.Model.Shop;
using System.ComponentModel.DataAnnotations.Schema;

namespace MadeHuman_Server.Model.Outbound
{
    public class OutboundTaskItemDetails
    {
        public Guid Id { get; set; }
        public  int Quantity { get; set; }
        public int QuantityChecked { get; set; } = 0;
        public bool IsChecked { get; set; } = false;

        public Guid ProductSKUId { get; set; }
        public Guid OutboundTaskItemId { get; set; }
        [ForeignKey(nameof(ProductSKUId))]
        public ProductSKU ProductSKU { get; set; }
        [ForeignKey(nameof(OutboundTaskItemId))]
        public OutboundTaskItems OutboundTaskItems { get; set; }

    }
}
