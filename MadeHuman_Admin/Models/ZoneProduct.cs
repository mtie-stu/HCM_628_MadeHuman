namespace MadeHuman_Admin.Models
{
    public class ZoneProduct
    {
        public string Zone { get; set; }
        public string WarehouseLocationsCode { get; set; }
        public string InventoryID { get; set; }
        public string SKUID { get; set; }
        public int Quantity { get; set; }
    }
    public class ZoneManagementViewModel
    {
        public List<ZoneProduct> Products { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string SelectedZone { get; set; }
        public string SearchTerm { get; set; }
        public string BaseUrl { get; set; }
    }
}
