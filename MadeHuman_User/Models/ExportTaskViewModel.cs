namespace MadeHuman_User.Models
{
    /// <summary>
    /// ViewModel tổng cho quy trình xuất kho theo task
    /// </summary>
    public class ExportTaskViewModel
    {
        /// <summary>
        /// Mã nhiệm vụ xuất kho (task export) được giao xuống
        /// </summary>
        public string ExportCode { get; set; } = string.Empty;

        /// <summary>
        /// Danh sách sản phẩm cần xuất trong task này
        /// </summary>
        public List<ProductExportViewModel> Products { get; set; } = new();

        /// <summary>
        /// Mã vị trí quét thực tế (để đối chiếu)
        /// </summary>
        public string? ScannedLocationCode { get; set; }

        /// <summary>
        /// Mã sản phẩm quét thực tế (để đối chiếu)
        /// </summary>
        public string? ScannedProductCode { get; set; }

        /// <summary>
        /// Số lượng thực tế xuất kho
        /// </summary>
        public int ScannedQuantity { get; set; }
    }

    /// <summary>
    /// Thông tin sản phẩm thuộc nhiệm vụ xuất kho
    /// </summary>
    public class ProductExportViewModel
    {
        public string ProductItemId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string LocationStorage { get; set; } = string.Empty;
        public int Quantity { get; set; } // Số lượng cần xuất theo nhiệm vụ
    }
}
