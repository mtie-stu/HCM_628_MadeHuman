using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using MadeHuman_Server.Model.Shop;
using System.Text.Json.Serialization;


namespace MadeHuman_Server.Model.Inbound
{
    public class InboundReceiptItems
    {
        [Key]
        public Guid Id { get; set; } = default!;
        public int Quantity { get; set; } = default!;

        public Guid InboundReceiptId { get; set; } = default!;
        [ForeignKey(nameof(InboundReceiptId))]
        [JsonIgnore] // 👈 Thêm dòng này
        public InboundReceipts InboundReceipts { get; set; }
        public Guid ProductSKUId { get; set; } = default!;
        [ForeignKey(nameof(ProductSKUId))]
        public ProductSKU ProductSKUs { get; set; }
    }
}