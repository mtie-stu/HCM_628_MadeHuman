using Google.Apis.Drive.v3.Data;
using MadeHuman_Server.Model.Inbound;
using MadeHuman_Server.Model.User_Task;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MadeHuman_Server.Model.Outbound
{
    public enum StatusPickTask 
    { 
      Created,
      Recived,
      Finished
    }
    public class PickTasks
    {
        public Guid Id { get; set; }    
        public DateTime CreateAt { get; set; }
        public DateTime FinishAt { get; set; }
        public StatusPickTask Status { get; set; }

        public Guid? UsersTasksId { get; set; }  // KHÓA NGOẠI CHÍNH
        public UsersTasks UsersTasks { get; set; }

        [JsonIgnore]
        public List<PickTaskDetails> PickTaskDetails { get; set; } = new();
        public Guid OutboundTaskId { get; set; } // ⚠️ THÊM KHÓA NGOẠI RÕ RÀNG
        [ForeignKey(nameof(OutboundTaskId))]
        public OutboundTask OutboundTask { get; set; }
        public Guid? OutboundTaskItemId { get; set; } // ⚠️ THÊM KHÓA NGOẠI RÕ RÀNG
        [ForeignKey(nameof(OutboundTaskItemId))]
        public OutboundTaskItems OutboundTaskItems { get; set; }


    }
}
