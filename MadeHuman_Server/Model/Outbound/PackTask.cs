using MadeHuman_Server.Model.User_Task;
using System.ComponentModel.DataAnnotations.Schema;

namespace MadeHuman_Server.Model.Outbound
{
    public enum StatusPackTask { 
        Created,
        Recived,
        Finished
    }
    public class PackTask
    {
        public Guid Id { get; set; }    
        public string MadeAt {  get; set; } 
        public StatusPackTask StatusPackTask { get; set; }
        public DateTime FinishAt { get; set; }
        public Guid? UsersTasksId { get; set; }  // KHÓA NGOẠI CHÍNH
        public UsersTasks UsersTasks { get; set; }
        public Guid? OutboundTaskItemId { get; set; } // ⚠️ THÊM KHÓA NGOẠI RÕ RÀNG
        [ForeignKey(nameof(OutboundTaskItemId))]
        public OutboundTaskItems OutboundTaskItems { get; set; }



    }
}
