namespace MadeHuman_User.Models
{
    public class CheckerTaskMixViewModel
    {
        public string TaskId { get; set; }
        public string ProductItem { get; set; }
        public string LocationStorage { get; set; }
        public string Baskets { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; }
        public List<CheckerTaskItem> ProductTasks { get; set; }
    }

    public class CheckerTaskItem
    {
        public string Status { get; set; }
        public string SmallTaskID { get; set; }
        public string ProductItemID { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public string LocationStorage { get; set; }
    }
}
