using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MadeHuman_Server.Service.UserTask;
using MadeHuman_Server.Model.User_Task;


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
        public Guid Id { get; set; } = default!;
        public string CreateBy { get; set; } = string.Empty;
        public  DateTime CreateAt { get; set; } = default!;
        public Status Status { get; set; }
        public Guid UserId { get; set; } = default!;    
        public Guid InboundReceiptId { get; set; } = default!;
        [ForeignKey(nameof(InboundReceiptId))]
        [JsonIgnore]  // 👈 bỏ serialize property này để tránh lặp vô hạn
        public InboundReceipts InboundReceipts { get; set; }
        [JsonIgnore]
        public ICollection<ProductBatches> ProductBatches { get; set; }
        public Guid? UserTaskId { get; set; }

        [ForeignKey("UserTaskId")]
        public UsersTasks UsersTasks { get; set; }


    }
}