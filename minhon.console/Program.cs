// See https://aka.ms/new-console-template for more information
using IdentityModel.Client;
using minhon;
using Refit;
using static TexTra.APIAccessor_ja;

Console.WriteLine("Hello, World!");
var apiKey = Environment.GetEnvironmentVariable("API_KEY")!;
var apiSecret = Environment.GetEnvironmentVariable("API_SECRET")!;
var name = Environment.GetEnvironmentVariable("NAME")!;

HttpConnection.InitializeConnection(30, HttpConnection.ProxyType.None, "", 0, "", "");
var httpCon = new HttpConnection();
httpCon.Initialize(apiKey, apiSecret, "", "", "");

Dictionary<string, string> param = new()
{
    {"key", apiKey},
    {"name", name},
    {"text", "こんにちは"},
    {"type", "json"},
};
string content = "";
httpCon.GetContent("POST", new Uri("https://mt-auto-minhon-mlt.ucri.jgn-x.jp/api/mt/generalNT_ja_en/"), param, ref content, null);
Console.WriteLine(content);

// var httpClient = new HttpClient();
// var response = await httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
// {
//     Address = "https://demo.identityserver.io/connect/token",
//     ClientId = "client",
//     ClientSecret = "secret"
// });

var client = RestService.For<IMachineTranslationClient>("https://mt-auto-minhon-mlt.ucri.jgn-x.jp", new RefitSettings
{
    HttpMessageHandlerFactory = () => new OAuthHanlder(apiKey, apiSecret, new LoggingHandler(new HttpClientHandler())),
});

var res = await client.Translate(minhon.Language.ja, minhon.Language.en, new TranslateRequest(apiKey, name, "こんにちは"));
Console.WriteLine(res);
