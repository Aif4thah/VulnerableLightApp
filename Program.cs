using System.Web;
using Microsoft.AspNetCore.OpenApi;
using Swashbuckle.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using VulnerableWebApplication.VLAController;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.HttpOverrides;
using VulnerableWebApplication.VLAModel;


// Configuration :

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAntiforgery();

builder.Services.AddHttpLogging(logging =>
{
    logging.LoggingFields = HttpLoggingFields.All;
    logging.RequestHeaders.Add("X-Real-IP");
    logging.RequestBodyLogLimit = 4096;
    logging.ResponseBodyLogLimit = 4096;
    logging.CombineLogs = true;
});

var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();

var app = builder.Build();
app.UseAntiforgery();
app.UseMiddleware<XRealIPMiddleware>();
app.UseHttpLogging();
app.UseSwagger();
app.UseSwaggerUI();


// Variables :

var Secret = configuration["Secret"];
var LogFile = configuration["LogFile"];


// Endpoints :

app.MapGet("/", async (string? lang) => await Task.FromResult(VLAController.VulnerableHelloWorld(HttpUtility.UrlDecode(lang)))).WithOpenApi();

app.MapGet("/Xml", async (string i) => await Task.FromResult(VLAController.VulnerableXmlParser(HttpUtility.UrlDecode(i)))).WithOpenApi();

app.MapGet("/Json", async (string i) => await Task.FromResult(VLAController.VulnerableDeserialize(HttpUtility.UrlDecode(i)))).WithOpenApi();

app.MapGet("/Req", async (string? i) => await VLAController.VulnerableWebRequest(i)).WithOpenApi();

app.MapGet("/Addr", async (string i, string t) => await Task.FromResult(VLAController.VulnerableObjectReference(i, t, Secret))).WithOpenApi();

app.MapGet("/Dns", async (string i) => await Task.FromResult(VLAController.VulnerableCmd(HttpUtility.UrlDecode(i)))).WithOpenApi();

app.MapGet("/NoSQL", async (string s) => await Task.FromResult(VLAController.VulnerableNoSQL(HttpUtility.UrlDecode(s)))).WithOpenApi();

app.MapPost("/Auth", [ProducesResponseType(StatusCodes.Status200OK)] async (HttpRequest request, [FromBody]VulnerableWebApplication.VLAModel.Creds login) => await Task.FromResult(VLAController.VulnerableQuery(login.User, login.Passwd, Secret, LogFile)).Result).WithOpenApi();

app.MapPatch("/Patch", async ([FromForm]IFormFile file, [FromHeader(Name="X-Forwarded-For")] string h, string t) => await VLAController.VulnerableHandleFileUpload(file, h, t, Secret, LogFile)).DisableAntiforgery();

// Arguments :

string url = args.FirstOrDefault(arg => arg.StartsWith("--url="));

if (string.IsNullOrEmpty(url))
{
    app.Urls.Add("http://localhost:4000");
    app.Urls.Add("https://localhost:3000");
}
else app.Urls.Add(url.Replace("--url=",""));


// Lancement :

app.Run();
