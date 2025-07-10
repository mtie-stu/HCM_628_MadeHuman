namespace MadeHuman_Admin.Models.Outbound
{
    public class PickerViewModel
    {
        public int TaskId { get; set; }
        public string ProductItem { get; set; }
        public string LocationStorage { get; set; }
        public string Baskets { get; set; }
        public int? Quantity { get; set; }

        public string ProductName { get; set; }
        public string ImageUrl { get; set; }
    }
}
