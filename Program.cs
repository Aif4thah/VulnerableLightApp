using VulnerableWebApplication;
using System.Web;
using Microsoft.AspNetCore.OpenApi;
using Swashbuckle.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAntiforgery();

var app = builder.Build();
app.UseAntiforgery();
app.UseSwagger();
app.UseSwaggerUI();


app.MapGet("/", async (string? lang) => await Task.FromResult(VulnerableClass.VulnerableHelloWorld(HttpUtility.UrlDecode(lang)))).WithOpenApi();

app.MapGet("/Xml", async (string i) => await Task.FromResult(VulnerableClass.VulnerableXmlParser(HttpUtility.UrlDecode(i)))).WithOpenApi();

app.MapGet("/Json", async (string i) => await Task.FromResult(VulnerableClass.VulnerableDeserialize(HttpUtility.UrlDecode(i)))).WithOpenApi();

app.MapPost("/Auth", [ProducesResponseType(StatusCodes.Status200OK)] async (HttpRequest request, [FromBody] VulnerableClass.Creds login) => await Task.FromResult(VulnerableClass.VulnerableQuery(login.user, login.passwd)).Result).WithOpenApi();

app.MapGet("/Jwt", async (string i) => await Task.FromResult(VulnerableClass.VulnerableValidateToken(i))).WithOpenApi();

app.MapGet("/Req", async (string? i) => await VulnerableClass.VulnerableWebRequest(i)).WithOpenApi();

app.MapGet("/Addr", async (int i, string t) => await Task.FromResult(VulnerableClass.VulnerableObjectReference(i, t))).WithOpenApi();

app.MapGet("/Dns", async (string i) => await Task.FromResult(VulnerableClass.VulnerableCmd(HttpUtility.UrlDecode(i)))).WithOpenApi();

app.MapGet("/Rce", async (string i) => await Task.FromResult(VulnerableClass.VulnerableCodeExecution(HttpUtility.UrlDecode(i)))).WithOpenApi();

app.MapGet("/NoSQL", async (string s) => await Task.FromResult(VulnerableClass.VulnerableNoSQL(HttpUtility.UrlDecode(s)))).WithOpenApi();

app.MapGet("/Admin", [ProducesResponseType(StatusCodes.Status200OK)] async (string t, [FromHeader(Name = "X-Forwarded-For")] string h) => await Task.FromResult(Task.FromResult(VulnerableClass.VulnerableAdminDashboard(t, h)).Result));

app.MapPost("/Upload", async (IFormFile file) => await VulnerableClass.VulnerableHandleFileUpload(file)).DisableAntiforgery();


//!\ Change the API exposition below at your own risk /!\
app.Urls.Add("http://localhost:4000");
app.Urls.Add("https://localhost:3000");

app.Run();
