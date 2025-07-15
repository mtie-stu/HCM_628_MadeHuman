using MadeHuman_Server.Model.Inbound;
using MadeHuman_Server.Model.User_Task;
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
        public Guid UserTaskId { get; set; }
        public UsersTasks UsersTasks { get; set; }

        [JsonIgnore]
        public ICollection<PickTaskDetails> PickTaskDetails { get; set; }

    }
}
