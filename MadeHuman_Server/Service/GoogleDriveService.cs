using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Upload;
using Google.Apis.Drive.v3.Data;
using File = Google.Apis.Drive.v3.Data.File;

public class GoogleDriveService
{
    private readonly DriveService _driveService;
    private readonly string _folderId; // ID thư mục Drive

    public GoogleDriveService(IWebHostEnvironment env, IConfiguration config)
    {
        var credentialPath = Path.Combine(env.ContentRootPath, "Data", "credentials.json");

        GoogleCredential credential = GoogleCredential.FromFile(credentialPath)
            .CreateScoped(DriveService.Scope.Drive);

        _driveService = new DriveService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "MyDriveUploader"
        });

        _folderId = config["GoogleDrive:FolderId"];
    }

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        var fileMeta = new File
        {
            Name = file.FileName,
            Parents = new List<string> { _folderId }
        };

        using var stream = file.OpenReadStream();

        var request = _driveService.Files.Create(fileMeta, stream, file.ContentType);
        request.Fields = "id";

        var result = await request.UploadAsync();

        if (result.Status == UploadStatus.Completed)
        {
            var fileId = request.ResponseBody.Id;

            // make public
            await _driveService.Permissions.Create(new Permission
            {
                Type = "anyone",
                Role = "reader"
            }, fileId).ExecuteAsync();

            return $"https://drive.google.com/uc?id={fileId}";
        }

        throw new Exception("Upload failed");
    }
}
