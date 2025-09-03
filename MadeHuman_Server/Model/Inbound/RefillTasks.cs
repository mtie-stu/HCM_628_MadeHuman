using MadeHuman_Server.Model.Shop;
using MadeHuman_Server.Model.User_Task;
using MadeHuman_Server.Model.WareHouse;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace MadeHuman_Server.Model.Inbound
{
    public enum StatusRefillTasks
    {
        Incomplete,
        Completed,
        Canceled
    }
    public class RefillTasks
    {
        public Guid Id { get; set; }    
        public Guid? LowStockId { get; set; }
        public Guid? UserTaskId { get; set; }
        public DateTime CreateAt { get; set; }
        public StatusRefillTasks StatusRefillTasks { get; set; }    
        public string CreateBy { get; set; }    //UserId

        [ForeignKey("UserTaskId")]
        public UsersTasks UsersTasks { get; set; }
     
        [JsonIgnore]
        public ICollection<RefillTaskDetails> RefillTaskDetails { get; set; } // 1-n
        [ForeignKey("LowStockId")]
        public LowStockAlerts LowStockAlerts { get; set; }
    }
}
