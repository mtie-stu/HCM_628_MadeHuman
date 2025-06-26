namespace MadeHuman_Admin.Models.Outbound
{
    public class CheckerTaskSingleViewModel
    {
        public string TaskId { get; set; }
        public string ProductItemId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public string LocationStorage { get; set; }
        public string ImageUrl { get; set; }
        public string InputProductCode { get; set; }
        public int InputQuantity { get; set; }
    }
}
