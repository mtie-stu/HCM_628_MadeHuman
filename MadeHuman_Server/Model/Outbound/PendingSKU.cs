namespace MadeHuman_Server.Model.Outbound
{
    public class PendingSKU
    {
        public Guid Id { get; set; }
        public Guid CheckTaskId { get; set; }
        public string UserId { get; set; } = null!;
        public string SKU { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public CheckTasks CheckTask { get; set; } = null!;
    }

}
