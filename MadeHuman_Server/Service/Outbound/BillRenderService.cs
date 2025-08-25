using System.Text;
using System.Net;
using Microsoft.EntityFrameworkCore;
using MadeHuman_Server.Data;
using Madehuman_Share.ViewModel.Outbound;

namespace MadeHuman_Server.Service.Outbound
{
    public interface IBillRenderService
    {
        Task<List<PrintBillViewModel>> GenerateBillHtmlByCheckTaskIdAsync(Guid checkTaskId, CancellationToken ct = default);
        Task<List<PrintBillViewModel>> GenerateBillHtmlByCheckTaskDetailIdAsync(Guid checkTaskDetailId, CancellationToken ct = default);
    }

    public class BillRenderService : IBillRenderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<BillRenderService> _logger;

        public BillRenderService(ApplicationDbContext context, IWebHostEnvironment env, ILogger<BillRenderService> logger)
        {
            _context = context;
            _env = env;
            _logger = logger;
        }

        // ===== PUBLIC APIS =====================================================

        /// <summary>
        /// Render nhiều bill theo CheckTaskId. An toàn: không ném lỗi ra ngoài, trả [] nếu lỗi.
        /// </summary>
        public async Task<List<PrintBillViewModel>> GenerateBillHtmlByCheckTaskIdAsync(
            Guid checkTaskId,
            CancellationToken ct = default)
        {
            try
            {
                var details = await _context.CheckTaskDetails
                    .AsNoTracking()
                    .Where(x => x.CheckTaskId == checkTaskId)
                    .OrderBy(x => x.OrderIndex) // nếu không có OrderIndex thì bỏ dòng này
                    .ToListAsync(ct);

                if (details.Count == 0)
                {
                    _logger.LogWarning("Không có CheckTaskDetails cho CheckTaskId {CheckTaskId}", checkTaskId);
                    return [];
                }

                var templateContent = await GetBillTemplateAsync(ct);
                if (templateContent is null)
                {
                    _logger.LogWarning("Template bill không khả dụng, bỏ qua render cho CheckTaskId {CheckTaskId}", checkTaskId);
                    return [];
                }

                var result = new List<PrintBillViewModel>(details.Count);
                foreach (var detail in details)
                {
                    ct.ThrowIfCancellationRequested();

                    var outboundId = detail.OutboundTaskItemId == Guid.Empty
                        ? string.Empty
                        : detail.OutboundTaskItemId.ToString();

                    var safeOutboundId = WebUtility.HtmlEncode(outboundId);
                    var html = templateContent.Replace("{{OUTBOUND_TASK_ITEM_ID}}", safeOutboundId);

                    result.Add(new PrintBillViewModel
                    {
                        OutboundTaskItemId = outboundId,
                        HtmlContent = html
                    });
                }

                return result;
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Lỗi render bill cho CheckTaskId {CheckTaskId}, bỏ qua.", checkTaskId);
                return [];
            }
        }

        /// <summary>
        /// Render một bill theo CheckTaskDetailId. An toàn: không ném lỗi ra ngoài, trả [] nếu lỗi.
        /// </summary>
        public async Task<List<PrintBillViewModel>> GenerateBillHtmlByCheckTaskDetailIdAsync(
            Guid checkTaskDetailId,
            CancellationToken ct = default)
        {
            try
            {
                var detail = await _context.CheckTaskDetails
                    .AsNoTracking()
                    .Where(x => x.Id == checkTaskDetailId)
                    .Select(x => new { x.OutboundTaskItemId })
                    .FirstOrDefaultAsync(ct);

                if (detail is null)
                {
                    _logger.LogWarning("Không tìm thấy CheckTaskDetailId {CheckTaskDetailId}", checkTaskDetailId);
                    return [];
                }

                var outboundId = detail.OutboundTaskItemId == Guid.Empty
                    ? string.Empty
                    : detail.OutboundTaskItemId.ToString();

                var templateContent = await GetBillTemplateAsync(ct);
                if (templateContent is null)
                {
                    _logger.LogWarning("Template bill không khả dụng, bỏ qua render cho CheckTaskDetailId {CheckTaskDetailId}", checkTaskDetailId);
                    return [];
                }

                var safeOutboundId = WebUtility.HtmlEncode(outboundId);
                var html = templateContent.Replace("{{OUTBOUND_TASK_ITEM_ID}}", safeOutboundId);

                return new List<PrintBillViewModel>
                {
                    new PrintBillViewModel
                    {
                        OutboundTaskItemId = outboundId,
                        HtmlContent = html
                    }
                };
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Lỗi render bill cho CheckTaskDetailId {CheckTaskDetailId}, bỏ qua.", checkTaskDetailId);
                return [];
            }
        }

        // ===== PRIVATE HELPERS =================================================

        private string? _billTemplateCache;

        /// <summary>
        /// Đọc template từ ContentRootPath/Data/Bill/shoporder-bill.html (có cache).
        /// Nếu thiếu file, thử fallback EmbeddedResource (nếu đã khai báo trong .csproj).
        /// </summary>
        private async Task<string?> GetBillTemplateAsync(CancellationToken ct)
        {
            if (!string.IsNullOrEmpty(_billTemplateCache))
                return _billTemplateCache;

            var path = Path.Combine(_env.ContentRootPath, "Data", "Bill", "shoporder-bill.html");

            try
            {
                if (File.Exists(path))
                {
                    var content = await File.ReadAllTextAsync(path, Encoding.UTF8, ct);
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        _billTemplateCache = content;
                        return _billTemplateCache;
                    }
                    _logger.LogError("Nội dung template rỗng: {Path}", path);
                }
                else
                {
                    _logger.LogError("Template không tìm thấy tại {Path}", path);
                }
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Không thể đọc template tại {Path}", path);
            }

            // ===== Fallback: Embedded Resource (tuỳ chọn) =====
            // Thêm vào .csproj:
            // <ItemGroup>
            //   <EmbeddedResource Include="Data/Bill/shoporder-bill.html" />
            // </ItemGroup>
            try
            {
                var asm = typeof(BillRenderService).Assembly;
                var resName = asm.GetManifestResourceNames()
                                 .FirstOrDefault(n => n.EndsWith("Data.Bill.shoporder-bill.html", StringComparison.OrdinalIgnoreCase));
                if (resName != null)
                {
                    using var s = asm.GetManifestResourceStream(resName);
                    if (s != null)
                    {
                        using var r = new StreamReader(s, Encoding.UTF8);
                        var content = await r.ReadToEndAsync();
                        if (!string.IsNullOrWhiteSpace(content))
                        {
                            _billTemplateCache = content;
                            _logger.LogInformation("Đã dùng EmbeddedResource làm template fallback: {Res}", resName);
                            return _billTemplateCache;
                        }
                    }
                }
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Fallback EmbeddedResource thất bại.");
            }

            return null; // Caller sẽ xử lý việc trả [] thay vì ném lỗi





        }
    }
}
