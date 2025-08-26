using MadeHuman_Server.Model.Shop;

namespace MadeHuman_Server.Model.Outbound
{
    public enum StatusOutbountTask 
    { 
        Incomplete,
        Completed
    }
    public class OutboundTask
    {
        public  Guid Id { get; set; }
        public DateTime CreateAt { get; set; } 
        public StatusOutbountTask Status { get; set; }
        public Baskets Baskets { get; set; }
        public ICollection<OutboundTaskItems> OutboundTaskItems { get; set; }
        public ICollection<PickTasks> PickTasks { get; set; }


    }
}
