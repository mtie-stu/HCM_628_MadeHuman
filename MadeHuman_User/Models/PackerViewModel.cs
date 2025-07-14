using System.Collections.Generic;

namespace MadeHuman_Admin.Models
{
    public class PackerViewModel
    {
        public string BasketId { get; set; }                    // Mã rổ hàng (tức là QR được quét)
        public int CurrentPage { get; set; }                    // Trang hiện tại (cho phân trang)
        public int TotalPages { get; set; }                     // Tổng số trang (phân trang)

        public List<TaskItem> TaskItems { get; set; }           // Danh sách task con

        public class TaskItem
        {
            public string TaskId { get; set; }                  // Mã task con
            public string ProductItemId { get; set; }           // Mã sản phẩm
            public string ProductName { get; set; }             // Tên sản phẩm
            public int Quantity { get; set; }                   // Số lượng cần đóng gói
            public string LocationStorage { get; set; }         // Vị trí lấy hàng
            public string Status { get; set; }                  // Trạng thái: Done / Pending / InProgress
        }
    }
}
