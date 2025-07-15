namespace Madehuman_Share
{
    public class BasketViewModel
    {
        public Guid? Id { get; set; }
        public int Status { get; set; } // 0 = Empty, 1 = Selected
        public Guid? OutBoundTaskId { get; set; }
    }
}
