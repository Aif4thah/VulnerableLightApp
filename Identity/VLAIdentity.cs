using System.Data;
using System.Security.Claims;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;


namespace VulnerableWebApplication.VLAIdentity
{
    public class VLAIdentity
    {
        public static async Task<object> VulnerableQuery(string User, string Passwd, string Secret, string LogFile)
        {
            /*
            Authentifie les utilisateurs par login et mot de passe, et renvoie un token JWT si l'authentification a réussi
            */
            SHA256 Sha256Hash = SHA256.Create();
            byte[] Bytes = Sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(Passwd));
            StringBuilder stringbuilder = new StringBuilder();
            for (int i = 0; i < Bytes.Length; i++) stringbuilder.Append(Bytes[i].ToString("x2"));
            string Hash = stringbuilder.ToString();

            VLAController.VLAController.VulnerableLogs("login attempt for:\n" + User + "\n" + Passwd + "\n", LogFile);
            var DataSet = VLAModel.Data.GetDataSet();
            var Result = DataSet.Tables[0].Select("Passwd = '" + Hash + "' and User = '" + User + "'");

            return Result.Length > 0 ? Results.Ok(VulnerableGenerateToken(User, Secret)) : Results.Unauthorized();
        }

        public static string VulnerableGenerateToken(string User, string Secret)
        {
            /*
            Retourne un token JWT signé pour l'utilisateur passé en paramètre
            */
            var TokenHandler = new JwtSecurityTokenHandler();
            var Key = Encoding.ASCII.GetBytes(Secret);
            var TokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("Id", User) }),
                Expires = DateTime.UtcNow.AddDays(365),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Key), SecurityAlgorithms.HmacSha256Signature)
            };
            var Token = TokenHandler.CreateToken(TokenDescriptor);

            return TokenHandler.WriteToken(Token);
        }

        public static bool VulnerableValidateToken(string Token, string Secret)
        {
            /*
            Vérifie la validité du token JWT passé en paramètre
            */
            var TokenHandler = new JwtSecurityTokenHandler();
            var Key = Encoding.ASCII.GetBytes(Secret);
            bool Result = true;
            Token = Token.Substring("Bearer ".Length);

            try
            {
                var JwtSecurityToken = TokenHandler.ReadJwtToken(Token);
                if (JwtSecurityToken.Header.Alg == "HS256" && JwtSecurityToken.Header.Typ == "JWT")
                {
                    TokenHandler.ValidateToken(Token, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Key),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                    }, out SecurityToken validatedToken);

                    var JwtToken = (JwtSecurityToken)validatedToken;
                }
            }
            catch(Exception e) { Result = false; }

            return Result;
        }
    }
}
