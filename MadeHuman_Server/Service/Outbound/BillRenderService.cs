using MadeHuman_Server.Data;
using Madehuman_Share.ViewModel.Outbound;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace MadeHuman_Server.Service.Outbound
{
    public interface IBillRenderService
    {
        Task<List<PrintBillViewModel>> GenerateBillHtmlByCheckTaskIdAsync(
              Guid checkTaskId,
              CancellationToken ct = default);
        Task<List<PrintBillViewModel>> GenerateBillHtmlByCheckTaskDetailIdAsync(
   Guid checkTaskDetailId,
   CancellationToken ct = default);
    }
    public class BillRenderService : IBillRenderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public BillRenderService(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<List<PrintBillViewModel>> GenerateBillHtmlByCheckTaskIdAsync(
      Guid checkTaskId,
      CancellationToken ct = default)
        {
            // 1) Lấy chi tiết theo CheckTaskId (không tracking, có thứ tự nếu bạn dùng OrderIndex)
            var details = await _context.CheckTaskDetails
                .AsNoTracking()
                .Where(x => x.CheckTaskId == checkTaskId)
                .OrderBy(x => x.OrderIndex) // bỏ dòng này nếu không có OrderIndex
                .ToListAsync(ct);

            if (details.Count == 0)
                throw new InvalidOperationException($"Không có CheckTaskDetails cho CheckTaskId = {checkTaskId}");

            // 2) Xây đường dẫn template theo ContentRootPath
            var htmlTemplatePath = Path.Combine(_env.ContentRootPath, "Data", "Bill", "shoporder-bill.html");
            if (!File.Exists(htmlTemplatePath))
                throw new FileNotFoundException($"Template not found: {htmlTemplatePath}");

            // 3) Đọc file template (UTF-8) + có hỗ trợ huỷ
            string templateContent;
            try
            {
                templateContent = await File.ReadAllTextAsync(htmlTemplatePath, Encoding.UTF8, ct);
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                throw new IOException($"Không thể đọc template tại {htmlTemplatePath}", ex);
            }

            if (string.IsNullOrWhiteSpace(templateContent))
                throw new InvalidOperationException("Nội dung template rỗng.");

            // 4) Render từng trang
            var result = new List<PrintBillViewModel>(details.Count);
            foreach (var detail in details)
            {
                ct.ThrowIfCancellationRequested();

                var outboundId = detail.OutboundTaskItemId.ToString();
                var html = templateContent.Replace("{{OUTBOUND_TASK_ITEM_ID}}", outboundId);

                result.Add(new PrintBillViewModel
                {
                    OutboundTaskItemId = outboundId,
                    HtmlContent = html
                });
            }

            return result;
        }


        public async Task<List<PrintBillViewModel>> GenerateBillHtmlByCheckTaskDetailIdAsync(
     Guid checkTaskDetailId,
     CancellationToken ct = default)
        {
            var detail = await _context.CheckTaskDetails
                .AsNoTracking()
                .Where(x => x.Id == checkTaskDetailId)
                .Select(x => new { x.OutboundTaskItemId })   // kiểu Guid (không nullable)
                .FirstOrDefaultAsync(ct);

            if (detail == null)
                return new List<PrintBillViewModel>();

            // Vì là Guid (non-nullable), không dùng HasValue/Value
            var outboundId = detail.OutboundTaskItemId == Guid.Empty
                ? string.Empty
                : detail.OutboundTaskItemId.ToString();

            var htmlTemplatePath = Path.Combine(_env.ContentRootPath, "Data", "Bill", "shoporder-bill.html");
            if (!File.Exists(htmlTemplatePath))
                throw new FileNotFoundException($"Template not found: {htmlTemplatePath}");

            var templateContent = await File.ReadAllTextAsync(htmlTemplatePath, Encoding.UTF8, ct);
            if (string.IsNullOrWhiteSpace(templateContent))
                throw new InvalidOperationException("Nội dung template rỗng.");

            var html = templateContent.Replace("{{OUTBOUND_TASK_ITEM_ID}}", outboundId);

            return new List<PrintBillViewModel>
    {
        new PrintBillViewModel
        {
            OutboundTaskItemId = outboundId,
            HtmlContent = html
        }
    };
        }



        //public async Task<List<PrintBillViewModel>> GenerateBillHtmlByCheckTaskDetailIdAsync(Guid checkTaskDetailId)
        //{
        //             var detail = await _context.CheckTaskDetails
        //        .FirstOrDefaultAsync(x => x.Id == checkTaskDetailId);

        //    if (detail == null)
        //        return new List<PrintBillViewModel>();

        //    var outboundId = detail.OutboundTaskItemId.ToString() ?? "";

        //    var htmlTemplatePath = Path.Combine(_env.ContentRootPath, "Data", "Bill", "shoporder-bill.html");
        //    var templateContent = await File.ReadAllTextAsync(htmlTemplatePath);

        //    var html = templateContent.Replace("{{OUTBOUND_TASK_ITEM_ID}}", outboundId);

        //    return new List<PrintBillViewModel>
        //    {
        //        new PrintBillViewModel
        //        {
        //            OutboundTaskItemId = outboundId,
        //            HtmlContent = html
        //        }
        //    };
        //}

    }
}
