using GestionEmpresarial.Application.Common.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestionEmpresarial.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly IEmailTemplateService _emailTemplateService;

        public EmailService(
            IConfiguration configuration, 
            ILogger<EmailService> logger,
            IEmailTemplateService emailTemplateService)
        {
            _configuration = configuration;
            _logger = logger;
            _emailTemplateService = emailTemplateService;
        }

        public async Task SendActivationEmailAsync(string email, string username, string activationToken)
        {
            try
            {
                _logger.LogInformation($"Iniciando envío de correo de activación a {email} con token {activationToken}");
                
                var template = await _emailTemplateService.GetDefaultTemplateByTypeAsync("AccountActivation");
                string subject;
                string body;

                // Generar un código de activación numérico de 6 dígitos basado en el token
                string activationCode = GenerateActivationCode(activationToken);
                _logger.LogInformation($"Código de activación generado: {activationCode}");

                if (template != null)
                {
                    _logger.LogInformation($"Plantilla encontrada: {template.Name}");
                    var variables = new Dictionary<string, string>
                    {
                        { "Username", username },
                        { "ActivationToken", activationToken },
                        { "ActivationUrl", $"{_configuration["AppUrl"]}/activate?token={activationToken}" },
                        { "ActivationCode", activationCode },
                        { "ActivationCodeUrl", $"{_configuration["AppUrl"]}/activate-code" }
                    };

                    subject = await _emailTemplateService.ProcessTemplateAsync(template.Subject, variables);
                    body = await _emailTemplateService.ProcessTemplateAsync(template.HtmlBody, variables);
                    _logger.LogInformation("Plantilla procesada correctamente");
                }
                else
                {
                    _logger.LogWarning("No se encontró plantilla de activación, usando plantilla por defecto");
                    // Plantilla por defecto si no hay ninguna en la base de datos
                    subject = "Activa tu cuenta en GestionEmpresarial";
                    body = $@"
                        <h1>¡Bienvenido a GestionEmpresarial, {username}!</h1>
                        <p>Gracias por registrarte. Para activar tu cuenta, haz clic en el siguiente enlace:</p>
                        <p><a href='{_configuration["AppUrl"]}/activate?token={activationToken}'>Activar mi cuenta</a></p>
                        <p>Si el enlace no funciona, puedes usar el siguiente código de activación: <strong>{activationCode}</strong></p>
                        <p>Para activar con el código, visita: <a href='{_configuration["AppUrl"]}/activate-code'>{_configuration["AppUrl"]}/activate-code</a></p>
                    ";
                }

                await SendEmailAsync(email, subject, body);
                _logger.LogInformation($"Correo de activación enviado correctamente a {email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al enviar correo de activación a {email}: {ex.Message}");
                // No lanzamos la excepción para evitar que falle el proceso principal
            }
        }

        public async Task SendRegistrationConfirmationEmailAsync(string email, string username)
        {
            var template = await _emailTemplateService.GetDefaultTemplateByTypeAsync("RegistrationConfirmation");
            string subject;
            string body;

            if (template != null)
            {
                var variables = new Dictionary<string, string>
                {
                    { "Username", username }
                };

                subject = await _emailTemplateService.ProcessTemplateAsync(template.Subject, variables);
                body = await _emailTemplateService.ProcessTemplateAsync(template.HtmlBody, variables);
            }
            else
            {
                // Plantilla por defecto si no hay ninguna en la base de datos
                subject = "Registro exitoso en GestionEmpresarial";
                body = $@"
                    <h1>¡Bienvenido a GestionEmpresarial, {username}!</h1>
                    <p>Tu cuenta ha sido creada exitosamente.</p>
                    <p>Ya puedes iniciar sesión y comenzar a utilizar nuestro sistema.</p>
                    <p>Saludos,<br>El equipo de GestionEmpresarial</p>
                ";
            }

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendPasswordResetEmailAsync(string email, string username, string resetToken)
        {
            var template = await _emailTemplateService.GetDefaultTemplateByTypeAsync("PasswordReset");
            string subject;
            string body;

            if (template != null)
            {
                var variables = new Dictionary<string, string>
                {
                    { "Username", username },
                    { "ResetToken", resetToken },
                    { "ResetUrl", $"{_configuration["AppUrl"]}/reset-password?token={resetToken}" }
                };

                subject = await _emailTemplateService.ProcessTemplateAsync(template.Subject, variables);
                body = await _emailTemplateService.ProcessTemplateAsync(template.HtmlBody, variables);
            }
            else
            {
                // Plantilla por defecto si no hay ninguna en la base de datos
                subject = "Restablecimiento de contraseña en GestionEmpresarial";
                body = $@"
                    <h1>Hola, {username}</h1>
                    <p>Has solicitado restablecer tu contraseña. Haz clic en el siguiente enlace para crear una nueva contraseña:</p>
                    <p><a href='{_configuration["AppUrl"]}/reset-password?token={resetToken}'>Restablecer mi contraseña</a></p>
                    <p>Si no has solicitado este cambio, puedes ignorar este correo.</p>
                    <p>Saludos,<br>El equipo de GestionEmpresarial</p>
                ";
            }

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendAccountUpdatedEmailAsync(string email, string username)
        {
            var template = await _emailTemplateService.GetDefaultTemplateByTypeAsync("AccountUpdated");
            string subject;
            string body;

            if (template != null)
            {
                var variables = new Dictionary<string, string>
                {
                    { "Username", username }
                };

                subject = await _emailTemplateService.ProcessTemplateAsync(template.Subject, variables);
                body = await _emailTemplateService.ProcessTemplateAsync(template.HtmlBody, variables);
            }
            else
            {
                // Plantilla por defecto si no hay ninguna en la base de datos
                subject = "Actualización de cuenta en GestionEmpresarial";
                body = $@"
                    <h1>Hola, {username}</h1>
                    <p>Te informamos que tu cuenta ha sido actualizada recientemente.</p>
                    <p>Si no has realizado estos cambios, por favor contacta con nuestro equipo de soporte inmediatamente.</p>
                    <p>Saludos,<br>El equipo de GestionEmpresarial</p>
                ";
            }

            await SendEmailAsync(email, subject, body);
        }

        private async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            try
            {
                _logger.LogInformation($"Preparando envío de correo a {to} con asunto: {subject}");
                
                var message = new MimeKit.MimeMessage();
                message.From.Add(new MimeKit.MailboxAddress(_configuration["EmailSettings:FromName"], _configuration["EmailSettings:FromEmail"]));
                message.To.Add(new MimeKit.MailboxAddress("", to));
                message.Subject = subject;

                var bodyBuilder = new MimeKit.BodyBuilder
                {
                    HtmlBody = htmlBody
                };

                message.Body = bodyBuilder.ToMessageBody();

                _logger.LogInformation($"Conectando al servidor SMTP: {_configuration["EmailSettings:SmtpServer"]}:{_configuration["EmailSettings:SmtpPort"]}");
                
                using var client = new SmtpClient();
                await client.ConnectAsync(
                    _configuration["EmailSettings:SmtpServer"],
                    int.Parse(_configuration["EmailSettings:SmtpPort"]),
                    SecureSocketOptions.StartTls);

                _logger.LogInformation($"Autenticando con usuario: {_configuration["EmailSettings:SmtpUsername"]}");
                
                await client.AuthenticateAsync(
                    _configuration["EmailSettings:SmtpUsername"],
                    _configuration["EmailSettings:SmtpPassword"]);

                _logger.LogInformation("Enviando correo...");
                await client.SendAsync(message);
                await client.DisconnectAsync(true);
                
                _logger.LogInformation($"Correo enviado exitosamente a {to} con asunto: {subject}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al enviar correo a {to}: {ex.Message}");
                // No lanzamos la excepción para evitar que falle el proceso principal
            }
        }

        // Método para generar un código de activación numérico de 6 dígitos basado en el token
        private string GenerateActivationCode(string token)
        {
            if (string.IsNullOrEmpty(token))
                return "000000";

            // Usar el token para generar un hash y convertirlo a un código numérico de 6 dígitos
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                var hashBytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(token));
                
                // Convertir los primeros bytes del hash a un número entero y tomar los últimos 6 dígitos
                var hashValue = BitConverter.ToInt32(hashBytes, 0);
                hashValue = Math.Abs(hashValue); // Asegurar que sea positivo
                
                // Asegurar que sea de 6 dígitos
                var code = (hashValue % 900000 + 100000).ToString();
                return code;
            }
        }
    }
}
