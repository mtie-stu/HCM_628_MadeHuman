using MadeHuman_Server.Model.Shop;
using MadeHuman_Server.Model.User_Task;
using System.ComponentModel.DataAnnotations.Schema;

namespace MadeHuman_Server.Model.Outbound
{
    public enum StatusCheckTask
    {
        Created,
        recived,
        finished
    }
    public class CheckTasks
    {
        public Guid Id { get; set; }    
        public DateTime CreateAt { get; set; }
        public string? MadeAt { get; set; }
        public StatusCheckTask StatusCheckTask { get; set; }

        public DateTime?  FinishAt { get; set; }

        public Guid? UsersTasksId { get; set; }  // KHÓA NGOẠI CHÍNH
        public UsersTasks UsersTasks { get; set; }

        //[JsonIgnore]
        //public List<PickTaskDetails> PickTaskDetails { get; set; } = new();
        public Guid OutboundTaskId { get; set; } // ⚠️ THÊM KHÓA NGOẠI RÕ RÀNG
        [ForeignKey(nameof(OutboundTaskId))]
        public OutboundTask OutboundTask { get; set; }
        public ICollection<CheckTaskDetails> CheckTaskDetails { get; set; }


    }
}
