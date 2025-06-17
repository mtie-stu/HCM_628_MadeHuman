namespace MadeHuman_Admin.Helper
{
    public static class PaginationHelper
    {
        public static string GeneratePagination(int currentPage, int totalPages, string baseUrl, string selectedZone = "", string searchTerm = "")
        {
            if (totalPages <= 1) return string.Empty;

            var builder = new System.Text.StringBuilder();
            builder.Append("<nav class=\"mt-4\" aria-label=\"Page navigation\">");
            builder.Append("<ul class=\"pagination justify-content-center\">");

            // Previous button
            if (currentPage > 1)
            {
                builder.AppendFormat(
                    "<li class=\"page-item\"><a class=\"page-link\" href=\"{0}?page={1}&zone={2}&searchTerm={3}\">Previous</a></li>",
                    baseUrl, currentPage - 1, selectedZone, searchTerm
                );
            }
            else
            {
                builder.Append("<li class=\"page-item disabled\"><span class=\"page-link\">Previous</span></li>");
            }

            // Page numbers (simple range, can expand logic later)
            for (int i = 1; i <= totalPages; i++)
            {
                if (i == currentPage)
                {
                    builder.AppendFormat(
                        "<li class=\"page-item active\"><span class=\"page-link\">{0}</span></li>", i
                    );
                }
                else
                {
                    builder.AppendFormat(
                        "<li class=\"page-item\"><a class=\"page-link\" href=\"{0}?page={1}&zone={2}&searchTerm={3}\">{1}</a></li>",
                        baseUrl, i, selectedZone, searchTerm
                    );
                }
            }

            // Next button
            if (currentPage < totalPages)
            {
                builder.AppendFormat(
                    "<li class=\"page-item\"><a class=\"page-link\" href=\"{0}?page={1}&zone={2}&searchTerm={3}\">Next</a></li>",
                    baseUrl, currentPage + 1, selectedZone, searchTerm
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
