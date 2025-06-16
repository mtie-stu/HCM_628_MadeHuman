using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;


namespace MadeHuman_Server.Model.Inbound
{
    public class InboundReceipts
    {

        [Key]
        public Guid Id { get; set; }
        public DateTime ReceivedAt { get; set; }


        public InboundTasks InboundTasks {  get; set; }
    }
}