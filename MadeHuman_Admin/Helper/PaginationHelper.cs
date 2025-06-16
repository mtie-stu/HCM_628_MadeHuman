namespace MadeHuman_Admin.Helper
{
    public static class PaginationHelper
    {
        public static string GeneratePagination(int currentPage, int totalPages, string baseUrl)
        {
            if (totalPages <= 1) return string.Empty;

            var builder = new System.Text.StringBuilder();
            builder.Append("<nav class=\"flex items-center justify-start space-x-3 mt-10 text-xs text-gray-400 select-none\" aria-label=\"Pagination\">");

            // Previous
            if (currentPage > 1)
            {
                builder.Append($"<a href=\"{baseUrl}?page={currentPage - 1}\" class=\"flex items-center space-x-1 text-orange-500 font-semibold cursor-pointer\"><i class=\"fas fa-arrow-left\"></i><span>Previous</span></a>");
            }
            else
            {
                builder.Append("<span class=\"flex items-center space-x-1 text-gray-300 cursor-default\"><i class=\"fas fa-arrow-left\"></i><span>Previous</span></span>");
            }

            // Page numbers (chỉ hiển thị 1,2,3,... logic tùy chỉnh)
            for (int i = 1; i <= totalPages; i++)
            {
                if (i == currentPage)
                {
                    builder.Append($"<span class=\"bg-orange-500 text-white rounded px-3 py-1 font-semibold cursor-pointer\">{i}</span>");
                }
                else
                {
                    builder.Append($"<a href=\"{baseUrl}?page={i}\" class=\"cursor-pointer hover:underline\">{i}</a>");
                }
            }

            // Next
            if (currentPage < totalPages)
            {
                builder.Append($"<a href=\"{baseUrl}?page={currentPage + 1}\" class=\"flex items-center space-x-1 text-orange-500 font-semibold cursor-pointer\"><span>Next</span><i class=\"fas fa-arrow-right\"></i></a>");
            }
            else
            {
                builder.Append("<span class=\"flex items-center space-x-1 text-gray-300 cursor-default\"><span>Next</span><i class=\"fas fa-arrow-right\"></i></span>");
            }

            builder.Append("</nav>");
            return builder.ToString();
        }
    }
}
