using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Madehuman_Share.ViewModel.Shop
{
    public class GenerateRandomMultiSKUOrdersRequest
    {
        public int TotalQuantity { get; set; }                // Tổng số đơn hàng cần tạo
        public string AppUserId { get; set; } = string.Empty; // Người tạo đơn
        public Guid? CategoryId { get; set; }                 // Optional: lọc theo Category
        public bool IgnoreStock { get; set; } = false;        // Bỏ qua tồn kho khi test
    }

}
