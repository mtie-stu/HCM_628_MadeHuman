using Google.Apis.Drive.v3.Data;
using MadeHuman_Server.Model.Shop;
using System.ComponentModel.DataAnnotations.Schema;

namespace MadeHuman_Server.Model.Outbound
{
    public enum StatusCheckDetailTask
    {
        Created,
        finished,
        PendingSupport,
        Error
    }
    public class CheckTaskDetails
    {
        public Guid Id { get; set; }
        public DateTime CreateAt { get; set; }
        public int OrderIndex { get; set; } // để map với #1...#8

        public StatusCheckDetailTask StatusCheckDetailTask { get; set; } = StatusCheckDetailTask.Created;
        public DateTime FinishAt { get; set; }
        public Guid CheckTaskId { get; set; } // KHÔNG phải PicksTasksId

        [ForeignKey(nameof(CheckTaskId))]
        public CheckTasks CheckTasks { get; set; }

        public Guid OutboundTaskItemId { get; set; }
        [ForeignKey(nameof(OutboundTaskItemId))]
        public OutboundTaskItems OutboundTaskItems { get; set; }

        public int QuantityChecked { get; set; } = 0;
        public bool IsChecked { get; set; } = false;

        public string? Reason { get; set; }
    }
}
