using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;


namespace MadeHuman_Server.Model.Inbound
{

    public enum Status
    {
        Created,
        Completed
    }
    public class InboundTasks
    {


        [Key]
        public Guid Id { get; set; }
        public string CreateBy { get; set; }
        public  DateTime CreateAt { get; set; }
        public Status Status { get; set; }

        public Guid InboundReceiptId { get; set; }
        [ForeignKey(nameof(InboundReceiptId))]
        public InboundReceipts InboundReceipts { get; set; }
        public ProductBatches ProductBatches { get; set; }


    }
}