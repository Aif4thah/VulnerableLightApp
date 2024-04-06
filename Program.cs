using System.Web;
using Microsoft.AspNetCore.OpenApi;
using Swashbuckle.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using VulnerableWebApplication.VLAController;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAntiforgery();

var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json").Build();

var app = builder.Build();
app.UseAntiforgery();
app.UseSwagger();
app.UseSwaggerUI();

var Secret = configuration["Secret"];
var LogFile = configuration["LogFile"];

app.MapGet("/", async (string? lang) => await Task.FromResult(VLAController.VulnerableHelloWorld(HttpUtility.UrlDecode(lang)))).WithOpenApi();

app.MapGet("/Xml", async (string i) => await Task.FromResult(VLAController.VulnerableXmlParser(HttpUtility.UrlDecode(i)))).WithOpenApi();

app.MapGet("/Json", async (string i) => await Task.FromResult(VLAController.VulnerableDeserialize(HttpUtility.UrlDecode(i)))).WithOpenApi();

app.MapGet("/Req", async (string? i) => await VLAController.VulnerableWebRequest(i)).WithOpenApi();

app.MapGet("/Addr", async (int i, string t) => await Task.FromResult(VLAController.VulnerableObjectReference(i, t, Secret))).WithOpenApi();

app.MapGet("/Dns", async (string i) => await Task.FromResult(VLAController.VulnerableCmd(HttpUtility.UrlDecode(i)))).WithOpenApi();

app.MapGet("/Rce", async (string i) => await Task.FromResult(VLAController.VulnerableCodeExecution(HttpUtility.UrlDecode(i)))).WithOpenApi();

app.MapGet("/NoSQL", async (string s) => await Task.FromResult(VLAController.VulnerableNoSQL(HttpUtility.UrlDecode(s)))).WithOpenApi();

app.MapGet("/Admin", [ProducesResponseType(StatusCodes.Status200OK)] async (string t, [FromHeader(Name = "X-Forwarded-For")] string h) => await Task.FromResult<string>(Task.FromResult<string>(VulnerableWebApplication.VLAController.VLAController.VulnerableAdminDashboard(t, h, Secret, LogFile)).Result));


app.MapPost("/Upload", async (IFormFile file) => await VLAController.VulnerableHandleFileUpload(file)).DisableAntiforgery();

app.MapPost("/Auth", [ProducesResponseType(StatusCodes.Status200OK)] async (HttpRequest request, [FromBody] VulnerableWebApplication.VLAModel.Creds login) => await Task.FromResult(VLAController.VulnerableQuery(login.User, login.Passwd, Secret, LogFile)).Result).WithOpenApi();


string url = args.FirstOrDefault(arg => arg.StartsWith("--url="));

if (string.IsNullOrEmpty(url))
{
    app.Urls.Add("http://localhost:4000");
    app.Urls.Add("https://localhost:3000");
}
else app.Urls.Add(url.Replace("--url=",""));

app.Run();
