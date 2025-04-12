using GestionEmpresarial.Application.Common.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using System;
using System.Threading.Tasks;

namespace GestionEmpresarial.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendActivationEmailAsync(string email, string username, string activationToken)
        {
            var subject = "Activa tu cuenta en GestionEmpresarial";
            var body = $@"
                <h1>¡Bienvenido a GestionEmpresarial, {username}!</h1>
                <p>Gracias por registrarte. Para activar tu cuenta, haz clic en el siguiente enlace:</p>
                <p><a href='{_configuration["AppUrl"]}/activate?token={activationToken}'>Activar mi cuenta</a></p>
                <p>Si no has solicitado esta cuenta, puedes ignorar este correo.</p>
                <p>Saludos,<br>El equipo de GestionEmpresarial</p>
            ";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendRegistrationConfirmationEmailAsync(string email, string username)
        {
            var subject = "Registro exitoso en GestionEmpresarial";
            var body = $@"
                <h1>¡Bienvenido a GestionEmpresarial, {username}!</h1>
                <p>Tu cuenta ha sido creada exitosamente.</p>
                <p>Ya puedes iniciar sesión y comenzar a utilizar nuestro sistema.</p>
                <p>Saludos,<br>El equipo de GestionEmpresarial</p>
            ";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendPasswordResetEmailAsync(string email, string username, string resetToken)
        {
            var subject = "Restablecimiento de contraseña en GestionEmpresarial";
            var body = $@"
                <h1>Hola, {username}</h1>
                <p>Has solicitado restablecer tu contraseña. Haz clic en el siguiente enlace para crear una nueva contraseña:</p>
                <p><a href='{_configuration["AppUrl"]}/reset-password?token={resetToken}'>Restablecer mi contraseña</a></p>
                <p>Si no has solicitado este cambio, puedes ignorar este correo.</p>
                <p>Saludos,<br>El equipo de GestionEmpresarial</p>
            ";

            await SendEmailAsync(email, subject, body);
        }

        public async Task SendAccountUpdatedEmailAsync(string email, string username)
        {
            var subject = "Actualización de cuenta en GestionEmpresarial";
            var body = $@"
                <h1>Hola, {username}</h1>
                <p>Te informamos que tu cuenta ha sido actualizada recientemente.</p>
                <p>Si no has realizado estos cambios, por favor contacta con nuestro equipo de soporte inmediatamente.</p>
                <p>Saludos,<br>El equipo de GestionEmpresarial</p>
            ";

            await SendEmailAsync(email, subject, body);
        }

        private async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_configuration["EmailSettings:FromName"], _configuration["EmailSettings:FromEmail"]));
                message.To.Add(new MailboxAddress("", to));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = htmlBody
                };

                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                await client.ConnectAsync(
                    _configuration["EmailSettings:SmtpServer"],
                    int.Parse(_configuration["EmailSettings:SmtpPort"]),
                    SecureSocketOptions.StartTls);

                await client.AuthenticateAsync(
                    _configuration["EmailSettings:SmtpUsername"],
                    _configuration["EmailSettings:SmtpPassword"]);

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
                
                _logger.LogInformation($"Correo enviado a {to} con asunto: {subject}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al enviar correo a {to}: {ex.Message}");
                // No lanzamos la excepción para evitar que falle el proceso principal
            }
        }
    }
}
