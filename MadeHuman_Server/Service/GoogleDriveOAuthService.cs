using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Util.Store;
using Microsoft.AspNetCore.Http;
using System.Threading;
using File = Google.Apis.Drive.v3.Data.File;

public class GoogleDriveOAuthService
{
    private readonly DriveService _driveService;
    private readonly string _folderId;

    public GoogleDriveOAuthService(IWebHostEnvironment env, IConfiguration config)
    {
        string credentialPath = Path.Combine(env.ContentRootPath, "Data", "credentials_oauth.json");
        string tokenPath = Path.Combine(env.ContentRootPath, "Data", "token.json");

        using var stream = new FileStream(credentialPath, FileMode.Open, FileAccess.Read);

        var cred = GoogleWebAuthorizationBroker.AuthorizeAsync(
            GoogleClientSecrets.FromStream(stream).Secrets,
            new[] { DriveService.Scope.Drive },
            "user",
            CancellationToken.None,
            new FileDataStore(Path.GetDirectoryName(tokenPath), true)
        ).Result;

        _driveService = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = cred,
            ApplicationName = "MadeHumanUploader"
        });

        _folderId = config["GoogleDrive:FolderId"]; // Lấy FolderId từ appsettings.json
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

            // Make file public
            var permission = new Google.Apis.Drive.v3.Data.Permission
            {
                Type = "anyone",
                Role = "reader"
            };
            await _driveService.Permissions.Create(permission, fileId).ExecuteAsync();

            return $"https://drive.google.com/uc?id={fileId}";
        }

        throw new Exception($"Upload failed. Status: {result.Status}, Exception: {result.Exception?.Message}");
    }
}
