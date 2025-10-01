using Microsoft.Extensions.Options;
using VulnerableWebApplication.VLAIdentity;
using VulnerableWebApplication;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Web;

namespace VulnerableWebApplication.MidlWare
{
    public class XRealIPMiddleware
        {
        /*
        Ajoute le Header "X-Real-IP:<IP>" pour les logs de l'application
        */
        private readonly RequestDelegate _next;

        public XRealIPMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Request.Headers["X-Real-IP"] = context.Connection.RemoteIpAddress.ToString();
            await _next(context);
        }

    }


    public class ValidateJwtMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidateJwtMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IConfiguration configuration)
        {
            /*
             * Authentifie les utilisateur
             * 
             * Renvoi certains messages d'érreur
             */
            var path = context.Request.Path.Value;
            if (path.Contains("script", StringComparison.OrdinalIgnoreCase)) path = HttpUtility.HtmlEncode(path); //XSS protection ;)

            string authHeader = context.Request.Headers["Authorization"];

            string UnauthMsg = "<html><head><title>Accès interdit</title></head>" +
                               "<body><h1>Erreur 401 - Accès non autorisé</h1>" +
                               "<p>START HACKING !</p></body></html>";

            string NotFoundMsg = "<html><head><title>Page introuvable</title></head>" +
                                 "<body><h1>Erreur 404 - Page non trouvée</h1>" +
                                 "<p>La ressource " + path + " n'existe pas ou a été déplacée.</p></body></html>";

            if (path.Equals("/login", StringComparison.OrdinalIgnoreCase) || path.StartsWith("/swagger", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            if (string.IsNullOrEmpty(authHeader) || !VLAIdentity.VLAIdentity.VulnerableValidateToken(authHeader, configuration["Secret"]))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "text/html; charset=utf-8";
                await context.Response.WriteAsync(UnauthMsg, Encoding.UTF8);
                return;
            }
            /*
            if (path.StartsWith("/Patch", StringComparison.OrdinalIgnoreCase) && (string.IsNullOrEmpty(authHeader) || !VLAIdentity.VLAIdentity.VulnerableAdminValidateToken(authHeader, configuration["Secret"])))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "text/html; charset=utf-8";
                await context.Response.WriteAsync(UnauthMsg, Encoding.UTF8);
                return;
            }
            */

            await _next(context);

            // 🔹 Gestion du 404 après exécution du pipeline
            if (context.Response.StatusCode == StatusCodes.Status404NotFound)
            {
                context.Response.ContentType = "text/html; charset=utf-8";
                await context.Response.WriteAsync(NotFoundMsg, Encoding.UTF8);
            }
        }



    }

}
