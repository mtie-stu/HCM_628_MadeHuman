using System.Net;
using System.Net.Http;
using System.Text;
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
        private readonly ILogger<BillRenderService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public BillRenderService(
            ApplicationDbContext context,
            ILogger<BillRenderService> logger,
            IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        // =============== PUBLIC APIS ===================

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

                var template = await GetTemplateAsync(ct);
                if (template is null)
                {
                    _logger.LogWarning("Không lấy được template (mọi nguồn đều fail).");
                    return [];
                }

                var result = new List<PrintBillViewModel>(details.Count);
                foreach (var d in details)
                {
                    ct.ThrowIfCancellationRequested();

                    var outboundId = d.OutboundTaskItemId == Guid.Empty ? string.Empty : d.OutboundTaskItemId.ToString();
                    var safeOutboundId = WebUtility.HtmlEncode(outboundId);

                    var html = template.Replace("{{OUTBOUND_TASK_ITEM_ID}}", safeOutboundId);

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

                var outboundId = detail.OutboundTaskItemId == Guid.Empty ? string.Empty : detail.OutboundTaskItemId.ToString();
                var safeOutboundId = WebUtility.HtmlEncode(outboundId);

                var template = await GetTemplateAsync(ct);
                if (template is null)
                {
                    _logger.LogWarning("Không lấy được template (mọi nguồn đều fail).");
                    return [];
                }

                var html = template.Replace("{{OUTBOUND_TASK_ITEM_ID}}", safeOutboundId);

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

        // =============== TEMPLATE RESOLUTION (no filesystem) ===================

        private string? _cachedTemplate; // cache trong RAM

        /// <summary>
        /// Nguồn ưu tiên: ENV BILL_TEMPLATE_HTML -> ENV BILL_TEMPLATE_URL (HTTP) -> EmbeddedResource -> BuiltIn.
        /// Trả null chỉ khi mọi nguồn đều fail (thực tế luôn có BuiltIn).
        /// </summary>
        private async Task<string?> GetTemplateAsync(CancellationToken ct)
        {
            if (!string.IsNullOrEmpty(_cachedTemplate))
                return _cachedTemplate;

            // 1) ENV: BILL_TEMPLATE_HTML (ưu tiên cao nhất)
            try
            {
                var fromEnv = Environment.GetEnvironmentVariable("BILL_TEMPLATE_HTML");
                if (!string.IsNullOrWhiteSpace(fromEnv))
                {
                    _cachedTemplate = fromEnv;
                    _logger.LogInformation("Template lấy từ ENV BILL_TEMPLATE_HTML.");
                    return _cachedTemplate;
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Đọc ENV BILL_TEMPLATE_HTML thất bại.");
            }

            // 2) ENV: BILL_TEMPLATE_URL (tải 1 lần và cache)
            try
            {
                var url = Environment.GetEnvironmentVariable("BILL_TEMPLATE_URL");
                if (!string.IsNullOrWhiteSpace(url))
                {
                    var http = _httpClientFactory.CreateClient();
                    using var req = new HttpRequestMessage(HttpMethod.Get, url);
                    using var res = await http.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);
                    res.EnsureSuccessStatusCode();
                    var body = await res.Content.ReadAsStringAsync(ct);
                    if (!string.IsNullOrWhiteSpace(body))
                    {
                        _cachedTemplate = body;
                        _logger.LogInformation("Template tải từ URL: {Url}", url);
                        return _cachedTemplate;
                    }
                }
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Tải template từ BILL_TEMPLATE_URL thất bại.");
            }

            // 3) EmbeddedResource (nếu bạn bật trong .csproj)
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
                            _cachedTemplate = content;
                            _logger.LogInformation("Template dùng EmbeddedResource: {Res}", resName);
                            return _cachedTemplate;
                        }
                    }
                }
            }
            catch (OperationCanceledException) { throw; }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Đọc EmbeddedResource thất bại.");
            }

            // 4) Built-in (luôn có)
            _cachedTemplate = BuiltInTemplate;
            _logger.LogInformation("Template dùng Built-in (fallback cuối).");
            return _cachedTemplate;
        }

        // =============== BUILT-IN TEMPLATE (80x80mm, có placeholder) ============
        // Bạn có thể chỉnh sửa HTML này theo ý muốn. Placeholder: {{OUTBOUND_TASK_ITEM_ID}}
        private const string BuiltInTemplate = @"<!DOCTYPE html>
<html>
<head>
    <meta charset=""utf-8"" />
    <title>Phiếu xuất kho – MadeHuman</title>
    <style>
        @page { size: 80mm 80mm; margin: 0; }
        html, body { width: 80mm; height: 80mm; margin: 0; padding: 0; }
        body {
            font-family: Arial, sans-serif;
            box-sizing: border-box;
            padding: 0.2mm;
            display: flex;
            align-items: center;
            justify-content: center;
            position: relative;
        }
        .qr { width: 58mm; height: 58mm; display: flex; align-items: center; justify-content: center; }
        .qr img { width: 100%; height: 100%; object-fit: contain; }
        .brand { position: absolute; right: 1.2mm; bottom: 1.2mm; font-size: 9pt; }
    </style>
</head>
<body>
    <div class=""qr"">
        <img src=""https://api.qrserver.com/v1/create-qr-code/?data={{OUTBOUND_TASK_ITEM_ID}}&size=800x800&margin=0"" alt=""QR Code"" />
    </div>
    <div class=""brand"">MadeHuman</div>
</body>
</html>";
    }
}
