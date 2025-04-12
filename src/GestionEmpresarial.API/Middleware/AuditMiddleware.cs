using GestionEmpresarial.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace GestionEmpresarial.API.Middleware
{
    public class AuditMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AuditMiddleware> _logger;

        public AuditMiddleware(RequestDelegate next, ILogger<AuditMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ICurrentUserService currentUserService, IAuditService auditService)
        {
            // Almacenar la información de la solicitud original
            var originalBodyStream = context.Response.Body;
            var method = context.Request.Method;
            var path = context.Request.Path;
            var queryString = context.Request.QueryString.ToString();
            var ipAddress = context.Connection.RemoteIpAddress?.ToString();
            var userAgent = context.Request.Headers["User-Agent"].ToString();

            // Solo auditar las solicitudes POST, PUT, DELETE
            if (method == "POST" || method == "PUT" || method == "DELETE" || 
                (method == "GET" && path.ToString().Contains("/api/auth/")))
            {
                try
                {
                    // Capturar el cuerpo de la solicitud para solicitudes POST, PUT, DELETE
                    string requestBody = string.Empty;
                    if (method != "GET" && context.Request.ContentLength > 0)
                    {
                        context.Request.EnableBuffering();
                        using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, true, 1024, true))
                        {
                            requestBody = await reader.ReadToEndAsync();
                            context.Request.Body.Position = 0;
                        }
                    }

                    // Capturar la respuesta
                    using (var responseBody = new MemoryStream())
                    {
                        context.Response.Body = responseBody;

                        // Continuar con la solicitud
                        await _next(context);

                        // Capturar la respuesta después de que se haya procesado la solicitud
                        responseBody.Seek(0, SeekOrigin.Begin);
                        var responseText = await new StreamReader(responseBody).ReadToEndAsync();
                        responseBody.Seek(0, SeekOrigin.Begin);

                        // Copiar la respuesta al flujo de respuesta original
                        await responseBody.CopyToAsync(originalBodyStream);

                        // Registrar la auditoría para solicitudes exitosas (códigos 2xx)
                        if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
                        {
                            var userId = currentUserService.UserId;
                            var userName = currentUserService.Username;
                            var action = GetActionFromMethod(method, path);
                            var tableName = GetTableNameFromPath(path);

                            // Solo registrar si tenemos un usuario identificado o es una solicitud de autenticación
                            if (!string.IsNullOrEmpty(userId) || path.ToString().Contains("/api/auth/"))
                            {
                                await auditService.LogActionAsync(
                                    userId ?? "Anonymous",
                                    userName ?? "Anonymous",
                                    action,
                                    tableName,
                                    queryString, // Usar queryString como primaryKey
                                    requestBody, // Usar el cuerpo de la solicitud como oldValues
                                    responseText, // Usar la respuesta como newValues
                                    string.Empty, // No tenemos información sobre columnas afectadas
                                    ipAddress,
                                    userAgent);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error en el middleware de auditoría");
                    context.Response.Body = originalBodyStream;
                    await _next(context);
                }
            }
            else
            {
                // Para solicitudes que no son POST, PUT, DELETE, simplemente continuar
                context.Response.Body = originalBodyStream;
                await _next(context);
            }
        }

        private string GetActionFromMethod(string method, PathString path)
        {
            if (path.ToString().Contains("/api/auth/login"))
                return "Login";
            if (path.ToString().Contains("/api/auth/logout"))
                return "Logout";

            return method switch
            {
                "POST" => "Create",
                "PUT" => "Update",
                "DELETE" => "Delete",
                "GET" => "Read",
                _ => "Other"
            };
        }

        private string GetTableNameFromPath(PathString path)
        {
            var pathSegments = path.ToString().Split('/');
            if (pathSegments.Length >= 3)
            {
                return pathSegments[2]; // /api/{tableName}/...
            }
            return "Unknown";
        }
    }
}
