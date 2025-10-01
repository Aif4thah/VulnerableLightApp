using System.Data;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;
using System.Net.Http.Headers;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using System.Xml.Linq;
using System.Xml.Xsl;
using System.Runtime.InteropServices;
using System.Web;
using VulnerableWebApplication.VLAModel;

namespace VulnerableWebApplication.VLAController
{
    public class VLAController
    {
        private static string LogFile;

        public static void SetLogFile(string logFile)
        {
            LogFile = logFile;
        }

        public static object VulnerableHelloWorld(string FileName = "english")
        {
            /*
            Retourne le contenu du fichier correspondant à la langue choisie par l'utilisateur
            */
            if (string.IsNullOrEmpty(FileName)) FileName = "francais";
            while (FileName.Contains("../") || FileName.Contains("..\\")) FileName = FileName.Replace("../", "").Replace("..\\", "");

            return Results.Ok(File.ReadAllText(FileName));
        }

        public static object VulnerableDeserialize(string Json)
        {
            /*
            Deserialise les données JSON passées en paramètre.
            On enregistre les objets "employé" valides dans un fichier en lecture seule
            */
            string NewId = "-1";
            string HaveToBeEmpty = string.Empty;
            string ROFile = "NewEmployees.txt";

            if (!File.Exists(ROFile)) File.Create(ROFile).Dispose();
            File.SetAttributes(ROFile, FileAttributes.ReadOnly);

            JsonConvert.DeserializeObject<object>(Json, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });            
            Employee NewEmployee = JsonConvert.DeserializeObject<Employee>(Json);

            if (NewEmployee != null && !string.IsNullOrEmpty(NewEmployee.Address) && !string.IsNullOrEmpty(NewEmployee.Id))
            {
                HaveToBeEmpty = VulnerableBuffer(NewEmployee.Address);
                if (string.IsNullOrEmpty(HaveToBeEmpty))
                {
                    NewId = VulnerableCodeExecution(NewEmployee.Id);
                    File.SetAttributes(ROFile, FileAttributes.Normal);
                    using (StreamWriter sw = new StreamWriter(ROFile, true)) sw.Write(JsonConvert.SerializeObject(NewEmployee, Newtonsoft.Json.Formatting.Indented));
                    File.SetAttributes(ROFile, FileAttributes.ReadOnly);
                }
            }

            return new Dictionary<string, object>{ { "File Attributes", File.GetAttributes(ROFile).ToString() },{ "NewId", NewId }, { "Memory Integrity", string.IsNullOrEmpty(HaveToBeEmpty) }
};
        }

        public static string VulnerableXmlParser(string Xml)
        {
            /*
            Parse les contrats au format XML passées en paramètre et retourne son contenu
            */
            try
            {
                var Xsl = XDocument.Parse(Xml);
                var MyXslTrans = new XslCompiledTransform(enableDebug: true);
                var Settings = new XsltSettings();
                MyXslTrans.Load(Xsl.CreateReader(), Settings, null);
                var DocReader = XDocument.Parse("<doc></doc>").CreateReader();

                var Sb = new StringBuilder();
                var DocWriter = XmlWriter.Create(Sb, new XmlWriterSettings() { ConformanceLevel = ConformanceLevel.Fragment });
                MyXslTrans.Transform(DocReader, DocWriter);

                return Sb.ToString();
            }
            catch (Exception ex)
            {
                XmlReaderSettings ReaderSettings = new XmlReaderSettings();
                ReaderSettings.DtdProcessing = DtdProcessing.Parse;
                ReaderSettings.XmlResolver = new XmlUrlResolver();
                ReaderSettings.MaxCharactersFromEntities = 6000;

                using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(Xml)))
                {
                    XmlReader Reader = XmlReader.Create(stream, ReaderSettings);
                    var XmlDocument = new XmlDocument();
                    XmlDocument.XmlResolver = new XmlUrlResolver();
                    XmlDocument.Load(Reader);

                    return XmlDocument.InnerText;
                }
            }
        }

        public static async Task<object> VulnerableWebRequest(string Uri = "https://localhost:3000/")
        {
            /*
            Effectue une requête web sur la boucle locale
            */
            if (string.IsNullOrEmpty(Uri)) Uri = "https://localhost:3000/";
            if (Regex.IsMatch(Uri, @"^https://localhost"))
            {
                using HttpClient Client = new();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));

                var Resp = await exec(Client, Uri);
                static async Task<string> exec(HttpClient client, string uri)
                {
                    var Result = client.GetAsync(uri);
                    Result.Result.EnsureSuccessStatusCode();
                    return Result.Result.StatusCode.ToString();
                }
                return Results.Ok(Resp);
            }
            else return Results.Unauthorized();
        }

        public static object VulnerableObjectReference(string Id)
        {
            /*
            Retourne les informations liées à l'ID de l'utilisateur
            Permets aux employés de consulter leurs données personnelles
            */
            var Employee = Data.GetEmployees()?.Where(x => Id == x.Id)?.FirstOrDefault();

            return Results.Ok(JsonConvert.SerializeObject(Employee));
        }

        public static object VulnerableCmd(string UserStr)
        {
            /*
            Effectue une requête DNS pour le FQDN passé en paramètre
            */
            if (Regex.Match(UserStr, @"^(?:[a-zA-Z0-9_\-]+\.)+[a-zA-Z]{2,}(?:.{0,100})$").Success)
            {
                Process Cmd = new Process();
                Cmd.StartInfo.FileName = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "powershell" : "/bin/sh";
                Cmd.StartInfo.RedirectStandardInput = true;
                Cmd.StartInfo.RedirectStandardOutput = true;
                Cmd.StartInfo.CreateNoWindow = true;
                Cmd.StartInfo.UseShellExecute = false;
                Cmd.Start();
                Cmd.WaitForExit(200);
                Cmd.StandardInput.WriteLine("nslookup " + UserStr);
                Cmd.StandardInput.Flush();
                Cmd.StandardInput.Close();

                return Results.Ok(Cmd.StandardOutput.ReadToEnd());
            }
            else return Results.Unauthorized();
        }

        public static unsafe string VulnerableBuffer(string UserStr)
        {
            /*
            Limite les chaines à 50 caractères
            */
            int BuffSize = 50;
            char* Ptr = stackalloc char[BuffSize], Str = Ptr + BuffSize;
            foreach (var c in UserStr) *Ptr++ = c;

            return new string(Str);
        }

        public static string VulnerableCodeExecution(string UserStr)
        {
            /*
            Retourne un nouvel Id d'employé
            */
            string Result = string.Empty;
            if (UserStr.Length < 40 && !UserStr.Contains("class") && !UserStr.Contains("using"))
            {
                Result = CSharpScript.EvaluateAsync($"System.Math.Pow(2, {UserStr})")?.Result?.ToString();
            }

            return Result;
        }

        public static async Task<IResult> VulnerableHandleFileUpload(IFormFile UserFile, string Header)
        {
            /*
            Permets l'upload de fichier de type SVG
            */
            if (!Header.Contains("10.10.10.256")) return Results.Unauthorized();

            if (UserFile.FileName.EndsWith(".svg")) 
            {
                using var Stream = File.OpenWrite(UserFile.FileName);
                await UserFile.CopyToAsync(Stream);

                return Results.Ok(UserFile.FileName);
            }
            else return Results.BadRequest();
        }

        public static async Task<object> VulnerableLogic(int price, int qty, string owner, string client, string activity)
        {
            /*
            Vérifie les champs du formulaire et calcul le prix d'une prestation
            */
            int tva = 30;
            int FinalPrice;
            if (price > 0 && !string.IsNullOrEmpty(owner) && !string.IsNullOrEmpty(client) && !string.IsNullOrEmpty(activity))
            {
                FinalPrice = price * qty;
                FinalPrice += (FinalPrice * tva) / 100;

                return Results.Ok(new { FinalPrice = $"{FinalPrice}€" });

            }
            return Results.StatusCode(400);

        }

        

    }
}
