using MadeHuman_Server.Model.Shop;

namespace MadeHuman_Server.Model.Inbound
{
    public class RefillTaskDetails
    {
        public Guid Id { get; set; }
        public Guid FromLocation { get; set; }
        public Guid ToLocation { get; set; }  
        public int Quantity { get; set; }
        public Guid RefillTaskId { get; set; }
        public RefillTasks RefillTasks { get; set; } // FK



    }
}
