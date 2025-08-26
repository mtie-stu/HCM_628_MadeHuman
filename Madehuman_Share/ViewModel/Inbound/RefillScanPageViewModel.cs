using Madehuman_Share.ViewModel.Shop;
using Madehuman_Share.ViewModel.WareHouse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_Share.ViewModel.Inbound
{
    public class RefillScanPageViewModel
    {
        public RefillTaskFullViewModel Task { get; set; } = new();
        public ScanRefillTaskValidationRequest ScanRequest { get; set; } = new();
        public RefillTaskDetailWithHeaderViewModel TaskDetailFlat { get; set; } = new();
        // ✅ Thêm danh sách vị trí kho để truy xuất
        public List<WarehouseLocationInfoViewModel> WarehouseLocations { get; set; } = new();
        // ✅ Thêm thông tin sản phẩm để hiện ra sau khi quét SKU
        public ProductSKUInfoViewmodel? ProductInfo { get; set; } // ← bạn sẽ populate cái này từ service
    }
}
