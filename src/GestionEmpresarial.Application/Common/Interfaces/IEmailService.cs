using System.Threading.Tasks;

namespace GestionEmpresarial.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendActivationEmailAsync(string email, string username, string activationToken);
        Task SendRegistrationConfirmationEmailAsync(string email, string username);
        Task SendPasswordResetEmailAsync(string email, string username, string resetToken);
        Task SendAccountUpdatedEmailAsync(string email, string username);
    }
}
