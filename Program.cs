using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using VulnerableWebApplication.VLAController;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.HttpOverrides;
using VulnerableWebApplication.VLAModel;
using VulnerableWebApplication.VLAIdentity;
using VulnerableWebApplication.MidlWare;
using VulnerableWebApplication.TestCpu;
using Microsoft.AspNetCore.OpenApi;
using GraphQL.Types;
using GraphQL;


// Configuration :

var builder = WebApplication.CreateBuilder(args);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAntiforgery();

// GraphQL

builder.Services.AddSingleton<IClientService, ClientService>();
builder.Services.AddSingleton<ClientDetailsType>();
builder.Services.AddSingleton<ClientQuery>();
builder.Services.AddSingleton<ISchema, ClientDetailsSchema>();
builder.Services.AddGraphQL(b => b.AddAutoSchema<ClientQuery>().AddSystemTextJson());

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

app.MapGet("/", async (string? lang) => await Task.FromResult(VLAController.VulnerableHelloWorld(HttpUtility.UrlDecode(lang))));

app.MapPost("/Login", [ProducesResponseType(StatusCodes.Status200OK)] async (HttpRequest request, [FromBody] VulnerableWebApplication.VLAModel.Creds login) => await Task.FromResult(VLAIdentity.VulnerableQuery(login.User, login.Passwd, Secret, LogFile)).Result).WithOpenApi();

app.MapGet("/Contract", async (string i, [FromHeader(Name="Authorization")] string t) => await Task.FromResult(VLAController.VulnerableXmlParser(HttpUtility.UrlDecode(i), t, Secret))).WithOpenApi();

app.MapGet("/LocalWebQuery", async (string? i) => await VLAController.VulnerableWebRequest(i)).WithOpenApi();

app.MapGet("/Employee", async (string i, [FromHeader(Name="Authorization")] string t) => await Task.FromResult(VLAController.VulnerableObjectReference(i, t, Secret))).WithOpenApi();

app.MapGet("/NewEmployee", async (string i, [FromHeader(Name = "Authorization")] string t) => await Task.FromResult(VLAController.VulnerableDeserialize(HttpUtility.UrlDecode(i), t, Secret))).WithOpenApi();

app.MapGet("/LocalDNSResolver", async (string i, [FromHeader(Name="Authorization")] string t) => await Task.FromResult(VLAController.VulnerableCmd(HttpUtility.UrlDecode(i), t ,Secret))).WithOpenApi();

app.MapPatch("/Patch", async ([FromHeader(Name="X-Forwarded-For")] string h, [FromHeader(Name = "Authorization")] string t, [FromForm] IFormFile file) => await VLAController.VulnerableHandleFileUpload(file, h, t, Secret, LogFile)).DisableAntiforgery().WithOpenApi();

app.UseGraphQL<ISchema>("/Client");

app.UseGraphQLPlayground("/GraphQLUI", new GraphQL.Server.Ui.Playground.PlaygroundOptions{GraphQLEndPoint="/Client",SubscriptionsEndPoint="/Client"});


// Arguments :

string url = args.FirstOrDefault(arg => arg.StartsWith("--url="));
string test = args.FirstOrDefault(arg => arg.StartsWith("--test"));

if(!string.IsNullOrEmpty(test))
{
    Console.WriteLine("Start CPU Testing");
    TestCpu.TestAffinity(Secret);
}



if (string.IsNullOrEmpty(url))
{
    app.Urls.Add("http://localhost:4000");
    app.Urls.Add("https://localhost:3000");
}
else app.Urls.Add(url.Substring("--url=".Length));

// Lancement :

app.Run();
