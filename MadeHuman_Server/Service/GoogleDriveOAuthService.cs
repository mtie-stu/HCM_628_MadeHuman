using System.Text;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

// ⚠️ Alias để tránh đụng tên với System.IO.File
using DriveFile = Google.Apis.Drive.v3.Data.File;

public class GoogleDriveOAuthService
{
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _config;
    private readonly ILogger<GoogleDriveOAuthService> _logger;
    private readonly IDataStore _dataStore;

    private DriveService? _drive;
    private readonly SemaphoreSlim _gate = new(1, 1);
    private const string UserKey = "main-user";

    public GoogleDriveOAuthService(
        IWebHostEnvironment env,
        IConfiguration config,
        ILogger<GoogleDriveOAuthService> logger,
        IDataStore dataStore)
    {
        _env = env;
        _config = config;
        _logger = logger;
        _dataStore = dataStore;
    }

    private string FolderId =>
        _config["GoogleDrive:FolderId"] ??
        _config["GoogleDrive__FolderId"] ??
        throw new InvalidOperationException("Missing GoogleDrive:FolderId / GoogleDrive__FolderId");

    private async Task EnsureDriveAsync(CancellationToken ct = default)
    {
        if (_drive != null) return;
        await _gate.WaitAsync(ct);
        try
        {
            if (_drive != null) return;

            // 1) Client secrets (ENV ưu tiên, fallback file)
            GoogleClientSecrets secrets;
            var credJson = _config["GoogleDrive:CredentialsJson"] ?? _config["GoogleDrive__CredentialsJson"];
            if (!string.IsNullOrWhiteSpace(credJson))
            {
                await using var ms = new MemoryStream(Encoding.UTF8.GetBytes(credJson));
                secrets = GoogleClientSecrets.FromStream(ms);
            }
            else
            {
                var credPath = Path.Combine(_env.ContentRootPath, "Data", "credentials_oauth.json");
                if (!System.IO.File.Exists(credPath))
                    throw new FileNotFoundException($"credentials_oauth.json not found at {credPath}");
                await using var fs = System.IO.File.OpenRead(credPath);
                secrets = GoogleClientSecrets.FromStream(fs);
            }

            // 2) Lấy token từ DB; nếu chưa có thì seed từ ENV/file rồi LƯU VÀO DB với TokenResponse
            var token = await _dataStore.GetAsync<TokenResponse>(UserKey);
            if (token == null)
            {
                var tokenJson = _config["GoogleDrive:TokenJson"] ?? _config["GoogleDrive__TokenJson"];
                if (string.IsNullOrWhiteSpace(tokenJson))
                {
                    var tokenPath = Path.Combine(_env.ContentRootPath, "Data", "Token.json"); // chú ý 'T' hoa nếu file bạn là vậy
                    if (!System.IO.File.Exists(tokenPath))
                        throw new FileNotFoundException(
                            "No token in DB and Token.json missing. Provide GoogleDrive:TokenJson (ENV) or add Data/Token.json");
                    tokenJson = await System.IO.File.ReadAllTextAsync(tokenPath, Encoding.UTF8, ct);
                }

                token = JsonConvert.DeserializeObject<TokenResponse>(tokenJson!)
                        ?? throw new InvalidOperationException("Invalid Token.json / TokenJson content.");

                // ✅ Sửa lỗi CS1503: lưu kiểu TokenResponse
                await _dataStore.StoreAsync<TokenResponse>(UserKey, token);
            }

            // 3) Flow + Credential dùng DB DataStore
            var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = secrets.Secrets,
                Scopes = new[] { DriveService.Scope.Drive },
                DataStore = _dataStore
            });

            var credential = new UserCredential(flow, UserKey, token);

            // 4) Thử refresh sớm để bắt invalid_grant sớm
            var refreshed = await credential.RefreshTokenAsync(ct);
            if (!refreshed)
                _logger.LogWarning("RefreshTokenAsync returned false; using current access token until expiry.");

            _drive = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "MadeHumanUploader"
            });

            _logger.LogInformation("Google Drive client initialized (DB-backed).");
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<string> UploadFileAsync(IFormFile file, CancellationToken ct = default)
    {
        await EnsureDriveAsync(ct);

        var meta = new DriveFile
        {
            Name = file.FileName,
            Parents = new[] { FolderId }
        };

        await using var stream = file.OpenReadStream();
        var req = _drive!.Files.Create(meta, stream, file.ContentType ?? "application/octet-stream");
        req.Fields = "id";

        var result = await req.UploadAsync(ct);
        if (result.Status != UploadStatus.Completed)
            throw new Exception($"Upload failed: {result.Exception?.Message}");

        var fileId = req.ResponseBody.Id;

        await _drive.Permissions.Create(new Permission { Type = "anyone", Role = "reader" }, fileId)
                                .ExecuteAsync(ct);

        return $"https://drive.google.com/uc?id={fileId}";
    }

    public async Task ClearCachedTokenAsync(CancellationToken ct = default)
    {
        await _dataStore.DeleteAsync<TokenResponse>(UserKey);
        _drive = null;
        _logger.LogInformation("Cleared cached Google OAuth token (DB).");
    }
}
