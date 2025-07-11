// ✅ GoogleDriveOAuthService.cs
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using Microsoft.AspNetCore.Http;
using File = Google.Apis.Drive.v3.Data.File;

public class GoogleDriveOAuthService
{
    private readonly DriveService _driveService;
    private readonly string _folderId;

    public GoogleDriveOAuthService(IWebHostEnvironment env, IConfiguration config)
    {
        string[] scopes = { DriveService.Scope.Drive }; // full access

        string tokenPath = Path.Combine(env.ContentRootPath, "Data", "token.json");
        _folderId = config["GoogleDrive:FolderId"];

        // ✅ Load token trực tiếp từ token.json (không dùng FileDataStore nữa)
        var credential = GoogleCredential.FromFile(tokenPath)
            .CreateScoped(scopes);

        _driveService = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "MadeHumanUploader"
        });
    }

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        var fileMetadata = new File
        {
            Name = file.FileName,
            Parents = new List<string> { _folderId }
        };

        using var stream = file.OpenReadStream();
        var request = _driveService.Files.Create(fileMetadata, stream, file.ContentType);
        request.Fields = "id";
        var result = await request.UploadAsync();

        if (result.Status == UploadStatus.Completed)
        {
            var fileId = request.ResponseBody.Id;

            await _driveService.Permissions.Create(new Google.Apis.Drive.v3.Data.Permission
            {
                Type = "anyone",
                Role = "reader"
            }, fileId).ExecuteAsync();

            return $"https://drive.google.com/uc?id={fileId}";
        }

        throw new Exception($"Upload failed: {result.Status}, {result.Exception?.Message}");
    }
}
