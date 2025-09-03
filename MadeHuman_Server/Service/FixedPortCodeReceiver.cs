// ✅ FixedPortCodeReceiver.cs
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Requests;
using Google.Apis.Auth.OAuth2.Responses;
using Google.Apis.Util;
using Google.Apis.Util.Store;
using System.Net;
using System.Text;

public class FixedPortCodeReceiver : ICodeReceiver
{
    private readonly string _redirectUri;
    public string RedirectUri => _redirectUri;

    public FixedPortCodeReceiver(string host = "localhost", int port = 8080)
    {
        _redirectUri = $"http://{host}:{port}/authorize/";
    }

    public async Task<AuthorizationCodeResponseUrl> ReceiveCodeAsync(AuthorizationCodeRequestUrl url, CancellationToken cancellationToken)
    {
        string authUrl = url.Build().ToString();
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = authUrl,
            UseShellExecute = true
        });

        var listener = new HttpListener();
        listener.Prefixes.Add(_redirectUri);
        listener.Start();

        var context = await listener.GetContextAsync();
        var response = context.Response;

        var query = context.Request.QueryString;
        string code = query["code"];
        string error = query["error"];

        var responseString = "<html><body>✅ Google login thành công. Bạn có thể đóng cửa sổ này.</body></html>";
        var buffer = Encoding.UTF8.GetBytes(responseString);
        response.ContentLength64 = buffer.Length;
        var responseOutput = response.OutputStream;
        await responseOutput.WriteAsync(buffer, 0, buffer.Length);
        responseOutput.Close();

        listener.Stop();

        return new AuthorizationCodeResponseUrl
        {
            Code = code,
            Error = error
        };
    }
}