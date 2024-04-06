using System.Data;
using System.Security.Claims;
using System.Text;
using System.Xml;
using Microsoft.AspNetCore.DataProtection;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System;
using System.Threading;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using static System.Net.Mime.MediaTypeNames;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Diagnostics.Tracing;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using System.Linq.Dynamic.Core;
using static VulnerableWebApplication.VLAController.VLAController;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using System.Xml.Xsl;
using VulnerableWebApplication.VLAModel;

namespace VulnerableWebApplication.VLAController
{
    public class VLAController
    {
        public static object VulnerableHelloWorld(string filename = "francais")
        {
            if (filename.IsNullOrEmpty()) filename = "francais";
            string content = File.ReadAllText(filename.Replace("../", "").Replace("..\\", ""));

            return "{\"success\":" + content + "}";
        }

        public static object VulnerableDeserialize(string json)
        {
            string f = "ReadOnly.txt";
            json = json.Replace("Framework", "").Replace("Token", "").Replace("cmd", "").Replace("powershell", "").Replace("http", "");
            if (!File.Exists(f)) File.WriteAllText(f, new Guid().ToString());
            File.SetAttributes(f, FileAttributes.ReadOnly);
            JsonConvert.DeserializeObject<object>(json, new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All });

            return "{\"" + f + "\":\"" + File.GetAttributes(f).ToString() + "\"}";
        }

        public static string VulnerableXmlParser(string xml)
        {
            try
            {
                var xsl = XDocument.Parse(xml);
                var myXslTrans = new XslCompiledTransform(enableDebug: true);
                var settings = new XsltSettings();
                myXslTrans.Load(xsl.CreateReader(), settings, null);
                var DocReader = XDocument.Parse("<doc></doc>").CreateReader();

                var sb = new StringBuilder();
                var DocWriter = XmlWriter.Create(sb, new XmlWriterSettings() { ConformanceLevel = ConformanceLevel.Fragment });
                myXslTrans.Transform(DocReader, DocWriter);

                return sb.ToString();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                xml = xml.Replace("Framework", "").Replace("Token", "").Replace("cmd", "").Replace("powershell", "").Replace("http", "");
                XmlReaderSettings ReaderSettings = new XmlReaderSettings();
                ReaderSettings.DtdProcessing = DtdProcessing.Parse;
                ReaderSettings.XmlResolver = new XmlUrlResolver();
                ReaderSettings.MaxCharactersFromEntities = 6000;

                using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
                {
                    XmlReader reader = XmlReader.Create(stream, ReaderSettings);
                    var xmlDocument = new XmlDocument();
                    xmlDocument.XmlResolver = new XmlUrlResolver();
                    xmlDocument.Load(reader);

                    return xmlDocument.InnerText;
                }
            }
        }

        public static string VulnerableLogs(string str, string logFile)
        {
            if (!File.Exists(logFile)) File.WriteAllText(logFile, Data.GetLogPage());
            string page = File.ReadAllText(logFile).Replace("</body>", "<p>" + str + "<p><br>" + Environment.NewLine + "</body>");
            File.WriteAllText(logFile, page);

            return "{\"success\":true}";
        }

        public static async Task<string> VulnerableQuery(string user, string passwd, string secret, string logFile)
        {
            SHA256 sha256Hash = SHA256.Create();
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(passwd));
            StringBuilder stringbuilder = new StringBuilder();

            for (int i = 0; i < bytes.Length; i++)
                stringbuilder.Append(bytes[i].ToString("x2"));
            string Hash = stringbuilder.ToString();

            VulnerableLogs("login attempt for:\n" + user + "\n" + passwd + "\n", logFile);
            var DataSet = Data.GetDataSet();
            var result = DataSet.Tables[0].Select("passwd = '" + Hash + "' and user = '" + user + "'");

            return result.Length > 0 ? "Bearer " + VulnerableGenerateToken(user, secret) : "{\"success\":false}";
        }

        public static string VulnerableGenerateToken(string user, string secret)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user) }),
                Expires = DateTime.UtcNow.AddDays(365),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public static bool VulnerableValidateToken(string token, string secret)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);
            bool result = true;
            try
            {
                var jwtSecurityToken = tokenHandler.ReadJwtToken(token);
                if (jwtSecurityToken.Header.Alg == "HS256" && jwtSecurityToken.Header.Typ == "JWT")
                {
                    tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                    }, out SecurityToken validatedToken);

                    var jwtToken = (JwtSecurityToken)validatedToken;
                }
            }
            catch { result = false; }

            return result;
        }

        public static async Task<string> VulnerableWebRequest(string uri = "https://localhost:3000/")
        {
            if (uri.IsNullOrEmpty()) uri = "https://localhost:3000/";
            if (Regex.IsMatch(uri, @"^https://localhost"))
            {
                using HttpClient client = new();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));

                var resp = await exec(client, uri);
                static async Task<string> exec(HttpClient client, string uri)
                {
                    var r = client.GetAsync(uri);
                    r.Result.EnsureSuccessStatusCode();
                    return r.Result.StatusCode.ToString();
                }
                return "{\"Result\":" + resp + "}";
            }
            else return "{\"Result\": \"Fordidden\"}";
        }

        public static string VulnerableObjectReference(int id, string token, string secret)
        {
            if (!VulnerableValidateToken(token, secret)) return "{\"" + id + "\":\"Forbidden\"}";
            else
            {
                List<Employee> Employees = Data.GetEmployees();
                return "{\"" + id + "\":\"" + Employees.Where(x => id == x.Id)?.FirstOrDefault()?.Address + "\"}";
            }
        }

        public static string VulnerableCmd(string i)
        {
            string r;
            if (Regex.Match(i, @"[a-zA-Z0-9][a-zA-Z0-9-]{1,10}\.[a-zA-Z]{2,3}$|[a-zA-Z0-9][a-zA-Z0-9-]{1,10}\.[a-zA-Z]{2,3}[& a-zA-Z]{2,10}$").Success)
            {
                Process cmd = new Process();
                cmd.StartInfo.FileName = "cmd.exe";
                cmd.StartInfo.RedirectStandardInput = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false;
                cmd.Start();
                cmd.WaitForExit(200);
                cmd.StandardInput.WriteLine("nslookup " + i);
                cmd.StandardInput.Flush();
                cmd.StandardInput.Close();

                r = "{\"result\":\"" + cmd.StandardOutput.ReadToEnd() + "\"}";
            }
            else r = "{\"result\":\"ERROR\"}";

            return r;
        }

        public static unsafe string VulnerableBuffer(string s)
        {
            char* ptr = stackalloc char[50], str = ptr + 50;
            foreach (var c in s) *ptr++ = c;

            return new string(str);
        }

        public static string VulnerableCodeExecution(string s)
        {
            string r = "null";
            if (s.Length < 40 && !s.Contains("class") && !s.Contains("=") && !s.Contains("using"))
            {
                try { r = CSharpScript.EvaluateAsync("System.Math.Pow(2, " + s + ")")?.Result?.ToString(); }
                catch (Exception e) { r = e.ToString(); }
            }

            return r + VulnerableBuffer(s);
        }

        public static string VulnerableNoSQL(string s)
        {
            if (s.Length > 250) return "{\"Result\": \"Fordidden\"}";
            List<Employee> Employees = Data.GetEmployees();
            var query = Employees.AsQueryable();
            var result = query.Where(s);

            return result.ToArray().ToString();
        }

        public static string VulnerableAdminDashboard(string token, string header, string secret, string logFile)
        {
            if (!VulnerableValidateToken(token, secret)) return "{\"Token\":\"Forbidden\"}";
            if (!header.Contains("10.256.256.256")) return "{\"IP\":\"Forbidden\"}";
            VulnerableLogs("admin logged with : " + token + header, logFile);

            return "{\"IP\":\"" + File.ReadAllText(logFile) + "\"}";
        }

        public static async Task<IResult> VulnerableHandleFileUpload(IFormFile file)
        {
            using var stream = File.OpenWrite(file.FileName);
            await file.CopyToAsync(stream);

            return Results.Ok(file.FileName);
        }
    }
}
