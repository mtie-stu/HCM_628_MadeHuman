namespace Madehuman_Share.ViewModel
{
    public class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
        public int CurrentPage { get; init; }
        public int PageSize { get; init; }
        public int TotalItems { get; init; }

        // Thống kê sau lọc (tuỳ chọn, giúp render "Tổng/Đang xử lý/Đã hoàn thành")
        public int TotalCompleted { get; init; }
        public int TotalIncomplete { get; init; }

        public int TotalPages
        {
            get
            {
                var size = PageSize > 0 ? PageSize : 1;
                var pages = (int)Math.Ceiling(TotalItems / (double)size);
                return pages < 1 ? 1 : pages;
            }
        }

        // Giữ filter
        public string? Status { get; init; }
        public string? Q { get; init; }

        // Tiện cho FE
        public bool HasPrev => CurrentPage > 1;
        public bool HasNext => CurrentPage < TotalPages;
    }
}
