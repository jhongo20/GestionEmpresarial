using GestionEmpresarial.API.Middleware;
using Microsoft.AspNetCore.Builder;

namespace GestionEmpresarial.API.Extensions
{
    public static class AuditMiddlewareExtensions
    {
        public static IApplicationBuilder UseAuditMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<AuditMiddleware>();
        }
    }
}
