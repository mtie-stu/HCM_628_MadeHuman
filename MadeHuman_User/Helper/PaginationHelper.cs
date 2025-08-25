using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.WebUtilities; // cho QueryHelpers

namespace MadeHuman_User.Helper
{
    public static class PaginationHelper
    {
        public static string GeneratePagination(
            int currentPage,
            int totalPages,
            string baseUrl,
            IDictionary<string, string>? queryParams = null,
            int maxPageDisplay = 5 // số trang hiển thị tối đa
        )
        {
            if (totalPages <= 1) return string.Empty;

            queryParams ??= new Dictionary<string, string>();

            var builder = new StringBuilder();
            builder.Append("<nav class=\"mt-4\" aria-label=\"Page navigation\">");
            builder.Append("<ul class=\"pagination justify-content-center\">");

            // Helper để tạo link với query params
            string BuildUrl(int page)
            {
                var qp = new Dictionary<string, string>(queryParams)
                {
                    ["page"] = page.ToString()
                };
                return QueryHelpers.AddQueryString(baseUrl, qp);
            }

            // Previous button
            if (currentPage > 1)
            {
                builder.AppendFormat(
                    "<li class=\"page-item\"><a class=\"page-link\" href=\"{0}\">Previous</a></li>",
                    BuildUrl(currentPage - 1)
                );
            }
            else
            {
                builder.Append("<li class=\"page-item disabled\"><span class=\"page-link\">Previous</span></li>");
            }

            // --- Page numbers with range ---
            int halfRange = maxPageDisplay / 2;
            int startPage = Math.Max(1, currentPage - halfRange);
            int endPage = Math.Min(totalPages, startPage + maxPageDisplay - 1);

            if (endPage - startPage + 1 < maxPageDisplay)
            {
                startPage = Math.Max(1, endPage - maxPageDisplay + 1);
            }

            if (startPage > 1)
            {
                builder.AppendFormat("<li class=\"page-item\"><a class=\"page-link\" href=\"{0}\">1</a></li>", BuildUrl(1));
                if (startPage > 2)
                {
                    builder.Append("<li class=\"page-item disabled\"><span class=\"page-link\">...</span></li>");
                }
            }

            for (int i = startPage; i <= endPage; i++)
            {
                if (i == currentPage)
                {
                    builder.AppendFormat("<li class=\"page-item active\"><span class=\"page-link\">{0}</span></li>", i);
                }
                else
                {
                    builder.AppendFormat("<li class=\"page-item\"><a class=\"page-link\" href=\"{0}\">{1}</a></li>", BuildUrl(i), i);
                }
            }

            if (endPage < totalPages)
            {
                if (endPage < totalPages - 1)
                {
                    builder.Append("<li class=\"page-item disabled\"><span class=\"page-link\">...</span></li>");
                }
                builder.AppendFormat("<li class=\"page-item\"><a class=\"page-link\" href=\"{0}\">{1}</a></li>", BuildUrl(totalPages), totalPages);
            }

            // Next button
            if (currentPage < totalPages)
            {
                builder.AppendFormat(
                    "<li class=\"page-item\"><a class=\"page-link\" href=\"{0}\">Next</a></li>",
                    BuildUrl(currentPage + 1)
                );
            }
            else
            {
                builder.Append("<li class=\"page-item disabled\"><span class=\"page-link\">Next</span></li>");
            }

            builder.Append("</ul></nav>");
            return builder.ToString();
        }
    }
}
