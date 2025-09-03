
namespace Madehuman_Share.ViewModel.Outbound
{
    public class ScanCheckTaskRequest
    {
        public Guid CheckTaskId { get; set; }
        public string SKU { get; set; } = string.Empty;
    }

    public class AssignSlotRequest
    {
        public Guid CheckTaskId { get; set; }
        public int SlotIndex { get; set; } // Tương ứng mã barcode #1..#8
    }

    public class CheckTaskResultViewModel
    {
        public bool Success { get; set; } = true;
        public List<string> Messages { get; set; } = new();
        public bool IsOrderCompleted { get; set; } = false;
        public bool IsTaskCompleted { get; set; } = false;
    }

    public class CheckTaskLogViewModel
    {
        public DateTime Time { get; set; }
        public string SKU { get; set; } = string.Empty;
        public int QuantityChanged { get; set; }
        public string? Note { get; set; }
        public string? PerformedBy { get; set; }
    }
}
