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
using System.Linq.Dynamic;
using static VulnerableWebApplication.VulnerableClass;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace VulnerableWebApplication
{
    public class VulnerableClass
    {
        public class Student { public int StudentID { get; set; } public string StudentName { get; set; } public int Age { get; set; } }
        public class Creds { public string user { get; set; } public string passwd { get; set; } }
        private static string secret { get; } = "FBC1534655BAD26AFF0C1F7C3113B3C521B9B635967831D1993ACEBB0D9A6129";

        public static string logFile = "Logs.html";

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
                settings.EnableScript = true;
                myXslTrans.Load(xsl.CreateReader(), settings, null);

                var doc = XDocument.Parse("<doc></doc>");
                var DocReader = doc.CreateReader();

                var sb = new StringBuilder();
                var DocWriter = XmlWriter.Create(sb, new XmlWriterSettings() { ConformanceLevel = ConformanceLevel.Fragment });
                myXslTrans.Transform(DocReader, DocWriter);
                return sb.ToString();
            }
            catch(Exception ex)
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

        public static string VulnerableLogs(string str)
        {
            string page = @"<!doctype html>
<html lang=""fr"">
<head>
<meta charset=""utf-8"">
<title>Application Logs</title>
</head>
<body>
<h1>Application Logs<h1>
</body>
</html>";
            if (!File.Exists(logFile)) File.WriteAllText(logFile, page);

            page = File.ReadAllText(logFile).Replace("</body>", "<p>" + str + "<p><br>" + Environment.NewLine + "</body>");
            File.WriteAllText(logFile, page);

            return "{\"success\":true}";

        }

        public static async Task<string> VulnerableQuery(string user, string passwd)
        {

            SHA256 sha256Hash = SHA256.Create();
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(passwd));
            StringBuilder stringbuilder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
                stringbuilder.Append(bytes[i].ToString("x2"));
            string Hash = stringbuilder.ToString();

            DataTable table = new DataTable();
            table.Columns.Add("user", typeof(string));
            table.Columns.Add("passwd", typeof(string));
            table.Rows.Add("root", "ce5ca673d13b36118d54a7cf13aeb0ca012383bf771e713421b4d1fd841f539a");
            table.Rows.Add("admin", "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918");
            table.Rows.Add("user", "e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855");
            var DataSet = new DataSet();
            DataSet.Tables.Add(table);

            VulnerableLogs("login attempt for:\n" + user + "\n" + passwd + "\n");
            var result = DataSet.Tables[0].Select("passwd = '" + Hash + "' and user = '" + user + "'");

            return result.Length > 0 ? "Bearer " + VulnerableGenerateToken(user) : "{\"success\":false}";
        }

        public static string VulnerableGenerateToken(string user)
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

        public static string VulnerableValidateToken(string token)
        {
            VulnerableLogs("token verification : "+ token );

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);
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
                    return "{\"success\":true}";
                }
                else return "{\"success\":true}";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "{\"success\":false}";
            }

        }

        public static async Task<string> VulnerableWebRequest(string uri="https://localhost:3000/")
        {
            if (uri.IsNullOrEmpty()) uri = "https://localhost:3000/";
            if (System.Text.RegularExpressions.Regex.IsMatch(uri, @"^https://localhost"))
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
            else { return "{\"Result\": \"Fordidden\"}"; }       
        }

        public static string VulnerableObjectReference(int id, string token)
        {
            if (VulnerableValidateToken(token).Contains("false"))
            {
                return "{\"" + id + "\":\"Forbidden\"}";
            }
            else
            {
                DataTable table = new DataTable();
                table.Columns.Add("id", typeof(int));
                table.Columns.Add("user", typeof(string));
                table.Columns.Add("adresse", typeof(string));
                table.Rows.Add(42, "admin", "3 rue Victor Hugo");
                table.Rows.Add(99, "root", "4 place Napoléon");
                var DataSet = new DataSet();
                DataSet.Tables.Add(table);

                var result = from t in table.AsEnumerable() where t.Field<int>("id").Equals(id) select t.Field<string>("adresse");

                return "{\"" + id + "\":\"" + result.FirstOrDefault() + "\"}";
            }
        }

        public static string VulnerableCmd(string i)
        {
            string r;
            if (Regex.Match(i, @"[a-zA-Z0-9][a-zA-Z0-9-]{1,10}\.[a-zA-Z]{2,3}$|[a-zA-Z0-9][a-zA-Z0-9-]{1,10}\.[a-zA-Z]{2,3}[& a-zA-Z]{2,10}$").Success)
            {
                string message = "nslookup " + i;
                int timeout = 200;
                System.Diagnostics.Process cmd = new Process();
                cmd.StartInfo.FileName = "cmd.exe";
                cmd.StartInfo.RedirectStandardInput = true;
                cmd.StartInfo.RedirectStandardOutput = true;
                cmd.StartInfo.CreateNoWindow = true;
                cmd.StartInfo.UseShellExecute = false;
                cmd.Start();
                cmd.WaitForExit(timeout);
                cmd.StandardInput.WriteLine(message);
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
            foreach (var c in s)
                *ptr++ = c;
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


        public static string VulnerableNoSQL(string f, string o, string v)
        {
            List<Student> Students = new List<Student>() {
                new Student() { StudentID = 1, StudentName = "John", Age = 16,},
                new Student() { StudentID = 2, StudentName = "Steve",  Age = 21 },
                new Student() { StudentID = 3, StudentName = "Bill",  Age = 18 },
                new Student() { StudentID = 4, StudentName = "Ram" , Age = 20},
                new Student() { StudentID = 5, StudentName = "Ron" , Age = 21}
           };
            var field = f;
            var op = o;
            var value = v;
            return Students.Where(field + op + value).FirstOrDefault().ToString();

        }

        public static string VulnerableAdminDashboard(string token, string header)
        {
            if (VulnerableValidateToken(token).Contains("false")) { return "{\"Token\":\"Forbidden\"}"; }
            if ( !header.Contains("10.256.256.256") ) { return "{\"IP\":\"Forbidden\"}"; }
            VulnerableLogs("admin logged with : " + token + header);
            return "{\"IP\":\""+ File.ReadAllText(logFile) + "\"}";
        }

        public static async Task<IResult> VulnerableHandleFileUpload(IFormFile file)
        {
            using var stream = File.OpenWrite(file.FileName);
            await file.CopyToAsync(stream);
            return Results.Ok(file.FileName);
        }


    }
}
