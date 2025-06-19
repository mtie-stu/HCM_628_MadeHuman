using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using MadeHuman_Server.Model.Shop;

namespace MadeHuman_Server.Model.Inbound
{
    public enum StatusProductBatches
    {
        UnStored,
        Stored


    }
    public class ProductBatches
    {

        [Key]
        public Guid Id { get; set; }
        public int Quantity { get; set; }

        public StatusProductBatches StatusProductBatches { get; set; }
        
        public ProductSKU ProductSKUs{ get; set; }

        [ForeignKey(nameof(InboundTasks))]
        public Guid InboundTaskId { get; set; }


        public InboundTasks InboundTasks { get; set; }

    }
}