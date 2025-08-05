using MadeHuman_Server.Data;
using Madehuman_Share.ViewModel.Outbound;
using Microsoft.EntityFrameworkCore;

namespace MadeHuman_Server.Service.Outbound
{
    public interface IBillRenderService
    {
        Task<List<PrintBillViewModel>> GenerateBillHtmlByCheckTaskIdAsync(Guid checkTaskId);
        Task<List<PrintBillViewModel>> GenerateBillHtmlByCheckTaskDetailIdAsync(Guid checkTaskDetailId);
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

        public async Task<List<PrintBillViewModel>> GenerateBillHtmlByCheckTaskIdAsync(Guid checkTaskId)
        {
            var details = await _context.CheckTaskDetails
                .Where(x => x.CheckTaskId == checkTaskId)
                .ToListAsync();

            var htmlTemplatePath = Path.Combine(_env.ContentRootPath, "Data", "Bill", "shoporder-bill.html");
            var templateContent = await File.ReadAllTextAsync(htmlTemplatePath);

            var result = new List<PrintBillViewModel>();

            foreach (var detail in details)
            {
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

        public async Task<List<PrintBillViewModel>> GenerateBillHtmlByCheckTaskDetailIdAsync(Guid checkTaskDetailId)
        {
            var detail = await _context.CheckTaskDetails
                .FirstOrDefaultAsync(x => x.Id == checkTaskDetailId);

            if (detail == null)
                return new List<PrintBillViewModel>();

            var outboundId = detail.OutboundTaskItemId.ToString() ?? "";

            var htmlTemplatePath = Path.Combine(_env.ContentRootPath, "Data", "Bill", "shoporder-bill.html");
            var templateContent = await File.ReadAllTextAsync(htmlTemplatePath);

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

    }
}
