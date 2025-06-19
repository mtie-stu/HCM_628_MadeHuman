using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using MadeHuman_Server.Model.Shop;


namespace MadeHuman_Server.Model.Inbound
{
    public class InboundReceiptItems
    {
        [Key]
        public Guid Id { get; set; }
        public int Quantity { get; set; }

        public Guid InboundReceiptId { get; set; }
        [ForeignKey(nameof(InboundReceiptId))]
        public InboundReceipts InboundReceipts { get; set; }
        public Guid ProductSKUId { get; set; }
        [ForeignKey(nameof(ProductSKUId))]
        public ProductSKU ProductSKUs { get; set; }
    }
}