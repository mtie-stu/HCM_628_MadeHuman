using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.WebUtilities; // cho QueryHelpers

namespace MadeHuman_User.Helper
{
    public static class PaginationHelper
    {
        /// <summary>
        /// Tạo pager dạng button (SPA) – dùng JS bắt [data-page] để gọi API và render lại.
        /// </summary>
        /// <param name="currentPage">Trang hiện tại (1-based)</param>
        /// <param name="totalPages">Tổng số trang (>=1)</param>
        /// <param name="maxPageDisplay">Số trang hiển thị tối đa trong dải</param>
        /// <param name="navId">Id của thẻ &lt;nav&gt; để tiện query/bind</param>
        /// <param name="showFirstLast">Hiển thị nút First/Last</param>
        /// <param name="prevText">Nhãn nút Previous</param>
        /// <param name="nextText">Nhãn nút Next</param>
        /// <param name="firstText">Nhãn nút First</param>
        /// <param name="lastText">Nhãn nút Last</param>
        public static string GeneratePaginationAjax(
                    int currentPage,
                    int totalPages,
                    int maxPageDisplay = 5,
                    string navId = "pager",
                    bool showFirstLast = true,
                    string prevText = "Previous",
                    string nextText = "Next",
                    string firstText = "First",
                    string lastText = "Last"
                )
                {
                    // Bảo vệ tổng trang
                    if (totalPages < 1) totalPages = 1;

                    // Nếu chỉ 1 trang -> không cần pager
                    if (totalPages <= 1) return string.Empty;

                    // Ép currentPage vào biên
                    if (currentPage < 1) currentPage = 1;
                    if (currentPage > totalPages) currentPage = totalPages;

                    var sb = new StringBuilder();
                    sb.Append($"""
        <nav id="{navId}" class="mt-4" aria-label="Page navigation" data-total-pages="{totalPages}">
          <ul class="pagination justify-content-center">
        """);

                    void Btn(int page, string text, bool disabled = false, bool active = false, string rel = "")
                    {
                        if (disabled)
                        {
                            sb.Append($"""    <li class="page-item disabled"><span class="page-link">{text}</span></li>""");
                            return;
                        }
                        var activeCls = active ? " active" : "";
                        var relAttr = string.IsNullOrEmpty(rel) ? "" : $""" rel="{rel}" """;
                        var aria = active ? """ aria-current="page" """ : "";
                        sb.Append($"""    <li class="page-item{activeCls}"><button type="button" class="page-link" data-page="{page}"{relAttr}{aria}>{text}</button></li>""");
                    }

                    // First
                    if (showFirstLast)
                        Btn(1, firstText, disabled: currentPage == 1, rel: "first");

                    // Previous
                    Btn(currentPage - 1, prevText, disabled: currentPage == 1, rel: "prev");

                    // Tính dải trang
                    if (maxPageDisplay < 3) maxPageDisplay = 3; // tránh quá hẹp
                    int half = maxPageDisplay / 2;
                    int start = System.Math.Max(1, currentPage - half);
                    int end = System.Math.Min(totalPages, start + maxPageDisplay - 1);
                    if (end - start + 1 < maxPageDisplay)
                        start = System.Math.Max(1, end - maxPageDisplay + 1);

                    // Trang đầu + dấu "..."
                    if (start > 1)
                    {
                        Btn(1, "1");
                        if (start > 2)
                            sb.Append("""    <li class="page-item disabled"><span class="page-link">...</span></li>""");
                    }

                    // Các trang trong dải
                    for (int i = start; i <= end; i++)
                        Btn(i, i.ToString(), active: i == currentPage);

                    // Dấu "..." + trang cuối
                    if (end < totalPages)
                    {
                        if (end < totalPages - 1)
                            sb.Append("""    <li class="page-item disabled"><span class="page-link">...</span></li>""");
                        Btn(totalPages, totalPages.ToString());
                    }

                    // Next
                    Btn(currentPage + 1, nextText, disabled: currentPage == totalPages, rel: "next");

                    // Last
                    if (showFirstLast)
                        Btn(totalPages, lastText, disabled: currentPage == totalPages, rel: "last");

                    sb.AppendLine();
                    sb.Append("""
          </ul>
        </nav>
        """);

                    return sb.ToString();
                }
        /// <summary>
        /// Pager SSR dùng <a href="..."> (tải lại trang). Hợp với phương án 2.
        /// </summary>
        public static string GeneratePagination(
            int currentPage,
            int totalPages,
            string baseUrl,
            IDictionary<string, string>? queryParams = null,
            int maxPageDisplay = 5,
            string prevText = "Previous",
            string nextText = "Next"
        )
        {
            if (totalPages < 1) totalPages = 1;
            if (totalPages <= 1) return string.Empty;

            if (currentPage < 1) currentPage = 1;
            if (currentPage > totalPages) currentPage = totalPages;

            // Lọc bỏ query rỗng cho URL gọn
            var clean = new Dictionary<string, string>();
            if (queryParams != null)
            {
                foreach (var kv in queryParams)
                    if (!string.IsNullOrWhiteSpace(kv.Value))
                        clean[kv.Key] = kv.Value!;
            }

            string BuildUrl(int page)
            {
                var qp = new Dictionary<string, string>(clean)
                {
                    ["page"] = page.ToString()
                };
                return QueryHelpers.AddQueryString(baseUrl, qp);
            }

            var sb = new StringBuilder();
            sb.Append("<nav class=\"mt-4\" aria-label=\"Page navigation\">");
            sb.Append("<ul class=\"pagination justify-content-center\">");

            // Prev
            if (currentPage > 1)
                sb.Append($"<li class=\"page-item\"><a class=\"page-link\" rel=\"prev\" href=\"{BuildUrl(currentPage - 1)}\">{prevText}</a></li>");
            else
                sb.Append($"<li class=\"page-item disabled\"><span class=\"page-link\">{prevText}</span></li>");

            // Range
            if (maxPageDisplay < 3) maxPageDisplay = 3;
            int half = maxPageDisplay / 2;
            int start = Math.Max(1, currentPage - half);
            int end = Math.Min(totalPages, start + maxPageDisplay - 1);
            if (end - start + 1 < maxPageDisplay)
                start = Math.Max(1, end - maxPageDisplay + 1);

            if (start > 1)
            {
                sb.Append($"<li class=\"page-item\"><a class=\"page-link\" href=\"{BuildUrl(1)}\">1</a></li>");
                if (start > 2)
                    sb.Append("<li class=\"page-item disabled\"><span class=\"page-link\">...</span></li>");
            }

            for (int i = start; i <= end; i++)
            {
                if (i == currentPage)
                    sb.Append($"<li class=\"page-item active\"><span class=\"page-link\" aria-current=\"page\">{i}</span></li>");
                else
                    sb.Append($"<li class=\"page-item\"><a class=\"page-link\" href=\"{BuildUrl(i)}\">{i}</a></li>");
            }

            if (end < totalPages)
            {
                if (end < totalPages - 1)
                    sb.Append("<li class=\"page-item disabled\"><span class=\"page-link\">...</span></li>");
                sb.Append($"<li class=\"page-item\"><a class=\"page-link\" href=\"{BuildUrl(totalPages)}\">{totalPages}</a></li>");
            }

            // Next
            if (currentPage < totalPages)
                sb.Append($"<li class=\"page-item\"><a class=\"page-link\" rel=\"next\" href=\"{BuildUrl(currentPage + 1)}\">{nextText}</a></li>");
            else
                sb.Append($"<li class=\"page-item disabled\"><span class=\"page-link\">{nextText}</span></li>");

            sb.Append("</ul></nav>");
            return sb.ToString();
        }
    }
}

