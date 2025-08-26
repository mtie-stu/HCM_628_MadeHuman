//using Google.Apis.Auth.OAuth2;
//using Google.Apis.Drive.v3;
//using Google.Apis.Services;
//using Google.Apis.Util.Store;
//using Google.Apis.Upload;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Configuration;
//using Microsoft.AspNetCore.Hosting;
//using System;
//using System.IO;
//using System.Threading;
//using System.Threading.Tasks;
//using File = Google.Apis.Drive.v3.Data.File;
//using Google.Apis.Drive.v3.Data;
//using Google.Apis.Auth.OAuth2.Flows;
//using Google.Apis.Auth.OAuth2.Responses;

//public class GoogleDriveOAuthService
//{
//    private readonly DriveService _driveService;
//    private readonly string _folderId;

//    public GoogleDriveOAuthService(IWebHostEnvironment env, IConfiguration config)
//    {
//        string credPath = Path.Combine(env.ContentRootPath, "Data", "credentials_oauth.json");
//        string tokenPath = Path.Combine(env.ContentRootPath, "Data", "token.json");

//        // 1. Load client secret
//        using var stream = new FileStream(credPath, FileMode.Open, FileAccess.Read);
//        var secrets = GoogleClientSecrets.FromStream(stream).Secrets;

//        // 2. Tải token đã có sẵn
//        var tokenJson = System.IO.File.ReadAllText(tokenPath);
//        var token = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenResponse>(tokenJson);

//        // 3. Tạo credential từ token
//        var credential = new UserCredential(
//            new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
//            {
//                ClientSecrets = secrets,
//                Scopes = new[] { DriveService.Scope.Drive }
//            }),
//            "user",
//            token
//        );

//        // 4. Khởi tạo Drive service
//        _driveService = new DriveService(new BaseClientService.Initializer
//        {
//            HttpClientInitializer = credential,
//            ApplicationName = "MadeHumanUploader"
//        });

//        _folderId = config["GoogleDrive:FolderId"];
//    }

//    public async Task<string> UploadFileAsync(IFormFile file)
//    {
//        var fileMeta = new File
//        {
//            Name = file.FileName,
//            Parents = new[] { _folderId }
//        };

//        using var stream = file.OpenReadStream();
//        var request = _driveService.Files.Create(fileMeta, stream, file.ContentType);
//        request.Fields = "id";

//        var result = await request.UploadAsync();

//        if (result.Status == UploadStatus.Completed)
//        {
//            string fileId = request.ResponseBody.Id;

//            // Mở quyền xem public
//            await _driveService.Permissions.Create(new Permission
//            {
//                Type = "anyone",
//                Role = "reader"
//            }, fileId).ExecuteAsync();

//            return $"https://drive.google.com/uc?id={fileId}";
//        }

//        throw new Exception($"Upload failed: {result.Exception?.Message}");
//    }
//}

//Source Tạo Token
//using Google.Apis.Auth.OAuth2;
//using Google.Apis.Drive.v3;
//using Google.Apis.Services;
//using Google.Apis.Util.Store;
//using Google.Apis.Upload;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Configuration;
//using Microsoft.AspNetCore.Hosting;
//using System;
//using System.IO;
//using System.Threading;
//using System.Threading.Tasks;
//using File = Google.Apis.Drive.v3.Data.File;
//using Google.Apis.Drive.v3.Data;
//using Google.Apis.Auth.OAuth2.Responses;
//using Google.Apis.Auth.OAuth2.Flows;

//public class GoogleDriveOAuthService
//{
//    private readonly DriveService _driveService;
//    private readonly string _folderId;

//    public GoogleDriveOAuthService(IWebHostEnvironment env, IConfiguration config)
//    {
//        string dataPath = Path.Combine(env.ContentRootPath, "Data");
//        string credPath = Path.Combine(dataPath, "credentials_oauth.json");
//        string tokenStorePath = dataPath;

//        Directory.CreateDirectory(dataPath);

//        // 1. Load client secret
//        using var stream = new FileStream(credPath, FileMode.Open, FileAccess.Read);
//        var secrets = GoogleClientSecrets.FromStream(stream).Secrets;

//        // ✅ 2. Dùng WebAuthorization để sinh token.json chứa refresh_token
//        var receiver = new FixedPortCodeReceiver("localhost", 8080);

//        var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
//            new ClientSecrets
//            {
//                ClientId = secrets.ClientId,
//                ClientSecret = secrets.ClientSecret
//            },
//            new[] { DriveService.Scope.Drive },
//            "user",
//            CancellationToken.None,
//            new FileDataStore(tokenStorePath, true),
//            receiver
//        ).Result;



//        Console.WriteLine("✅ Token đã được lưu tại: " + tokenStorePath);

//        // 3. Khởi tạo Drive service
//        _driveService = new DriveService(new BaseClientService.Initializer
//        {
//            HttpClientInitializer = credential,
//            ApplicationName = "MadeHumanUploader"
//        });

//        _folderId = config["GoogleDrive:FolderId"];
//    }

//    public async Task<string> UploadFileAsync(IFormFile file)
//    {
//        var fileMeta = new File
//        {
//            Name = file.FileName,
//            Parents = new[] { _folderId }
//        };

//        using var stream = file.OpenReadStream();
//        var request = _driveService.Files.Create(fileMeta, stream, file.ContentType);
//        request.Fields = "id";

//        var result = await request.UploadAsync();

//        if (result.Status == UploadStatus.Completed)
//        {
//            string fileId = request.ResponseBody.Id;

//            await _driveService.Permissions.Create(new Permission
//            {
//                Type = "anyone",
//                Role = "reader"
//            }, fileId).ExecuteAsync();

//            return $"https://drive.google.com/uc?id={fileId}";
//        }

//        throw new Exception($"Upload failed: {result.Exception?.Message}");
//    }
//}


using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Upload;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;
using File = Google.Apis.Drive.v3.Data.File;

public class GoogleDriveOAuthService
{
    private readonly DriveService _driveService;
    private readonly string _folderId;

    public GoogleDriveOAuthService(IWebHostEnvironment env, IConfiguration config)
    {
        string credPath = Path.Combine(env.ContentRootPath, "Data", "credentials_oauth.json");
        string tokenPath = Path.Combine(env.ContentRootPath, "Data", "token.json");

        // 1. Load client secrets
        using var stream = new FileStream(credPath, FileMode.Open, FileAccess.Read);
        var secrets = GoogleClientSecrets.FromStream(stream).Secrets;

        // 2. Load token.json từ file
        var tokenJson = System.IO.File.ReadAllText(tokenPath);
        var token = Newtonsoft.Json.JsonConvert.DeserializeObject<TokenResponse>(tokenJson);

        // 3. Tạo flow để SDK tự refresh
        var flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = secrets,
            Scopes = new[] { DriveService.Scope.Drive }
        });

        // 4. Tạo credential từ token đã lưu
        var credential = new UserCredential(flow, "user", token);

        // 5. Khởi tạo Drive API
        _driveService = new DriveService(new BaseClientService.Initializer
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
