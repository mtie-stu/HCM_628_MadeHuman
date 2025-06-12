namespace MadeHuman_Server.Model.Inbound
{
    public enum status
    {
        Delivered,
        Incomplete,
        Complete
    }
    public class InboundReceipt
    {
        public int Id { get; set; }
        public Guid ReceiptNumber { get; set; }
        public DateTime ReceivedAt { get; set; }
        public status Status { get; set; }

    }
}
