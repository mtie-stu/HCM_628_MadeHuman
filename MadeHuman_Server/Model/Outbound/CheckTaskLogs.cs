namespace MadeHuman_Server.Model.Outbound
{
    public class CheckTaskLogs
    {
        public Guid Id { get; set; }
        public DateTime Time { get; set; } = DateTime.UtcNow;

        public Guid CheckTaskId { get; set; }
        public CheckTasks CheckTask { get; set; }

        public string SKU { get; set; } = string.Empty;
        public int QuantityChanged { get; set; } = 1;

        public string? Note { get; set; } // ví dụ: thiếu hàng, sai SKU, hoàn tất đơn
        public string? PerformedBy { get; set; } // userId (redundant nhưng truy vết dễ)
    }

}
