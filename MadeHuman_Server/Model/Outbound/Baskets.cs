using Google.Apis.Drive.v3.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace MadeHuman_Server.Model.Outbound
{
    public enum StatusBaskets
    {
        Empty,
        Selected
    }
    public class Baskets
    {
        public Guid Id { get; set; }
        public StatusBaskets Status { get; set; }
        public Guid? OutBoundTaskId { get; set; }
        [ForeignKey(nameof(OutBoundTaskId))]
        public OutboundTask OutboundTask { get; set; }
    }
}
