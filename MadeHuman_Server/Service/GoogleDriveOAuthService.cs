using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.Upload;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using File = Google.Apis.Drive.v3.Data.File;
using Google.Apis.Drive.v3.Data;

public class GoogleDriveOAuthService
{
    private readonly DriveService _driveService;
    private readonly string _folderId;

    public GoogleDriveOAuthService(IWebHostEnvironment env, IConfiguration config)
    {
        // 1. Đường dẫn credentials và token
        string credPath = Path.Combine(env.ContentRootPath, "Data", "credentials_oauth.json");
        string tokenPath = Path.Combine(env.ContentRootPath, "Data", "token.json");

        // 2. Đọc client secrets
        using var stream = new FileStream(credPath, FileMode.Open, FileAccess.Read);
        var secrets = GoogleClientSecrets.FromStream(stream).Secrets;

        // 3. Tạo credential từ token có sẵn
        var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
            secrets,
            new[] { DriveService.Scope.Drive },
            "user",
            CancellationToken.None,
            new FileDataStore(Path.GetDirectoryName(tokenPath), true)
        ).Result;

        // 4. Tạo service
        _driveService = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "MadeHumanUploader"
        });

        _folderId = config["GoogleDrive:FolderId"];
    }

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        var fileMeta = new File
        {
            Name = file.FileName,
            Parents = new[] { _folderId }
        };

        using var stream = file.OpenReadStream();
        var request = _driveService.Files.Create(fileMeta, stream, file.ContentType);
        request.Fields = "id";

        var result = await request.UploadAsync();

        if (result.Status == UploadStatus.Completed)
        {
            string fileId = request.ResponseBody.Id;

            // Mở quyền xem public
            await _driveService.Permissions.Create(new Permission
            {
                Type = "anyone",
                Role = "reader"
            }, fileId).ExecuteAsync();

            return $"https://drive.google.com/uc?id={fileId}";
        }

        throw new Exception($"Upload failed: {result.Exception?.Message}");
    }
}
