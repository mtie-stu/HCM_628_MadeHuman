namespace MadeHuman_User.Models
{
    public class ImportTaskViewModel
    {
        public string TaskCode { get; set; } // Mã nhiệm vụ nhập hàng
        public string ScannedProductCode { get; set; } // Mã sản phẩm được quét
        public int? ScannedQuantity { get; set; } // Số lượng nhập kho
        public ProductImportViewModel? SelectedProduct { get; set; } // Sản phẩm được chọn sau khi quét
        public List<ProductImportViewModel> Products { get; set; } = new();
    }
    public class ProductImportViewModel
    {
        public string TaskCode { get; set; } // Mã nhiệm vụ
        public string ProductItemId { get; set; } // Mã sản phẩm
        public string ProductName { get; set; }
        public string LocationStorage { get; set; }
        public int Quantity { get; set; }
        public string ImageUrl { get; set; }
    }
}
