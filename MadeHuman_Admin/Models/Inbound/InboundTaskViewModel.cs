namespace MadeHuman_Admin.Models
{
    public class InboundTaskViewModel
    {
        public string InboundReceiptsID { get; set; }
        public string InboundReceiptsItemsID { get; set; }

        public string ProductItemId { get; set; }
        public string Quantity { get; set; }
        public string LocationStorage { get; set; }

        public string Zone { get; set; }
        public string WarehouseLocationsCode { get; set; }

        public string ProductImageUrl { get; set; } = "https://storage.googleapis.com/a1aa/image/bb8a5ce5-952e-4ad3-b719-429c2ebb7a23.jpg";
    }
}
