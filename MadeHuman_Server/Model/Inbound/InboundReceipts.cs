using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;


namespace MadeHuman_Server.Model.Inbound
{
    public enum StatusReceipt
    {
        Created,
        Confirmed,
        Received,
        Cancel
    }
    public class InboundReceipts
    {

        [Key]
        public Guid Id { get; set; } = default!;
        public DateTime ReceivedAt { get; set; } = default!;
        public DateTime CreateAt { get; set; } = default!;
        public StatusReceipt Status { get; set; }
        public ICollection<InboundReceiptItems> InboundReceiptItems { get; set; } = new List<InboundReceiptItems>();

        public InboundTasks InboundTasks {  get; set; }
    }
}