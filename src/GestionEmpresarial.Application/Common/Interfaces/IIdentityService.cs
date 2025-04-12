using GestionEmpresarial.Application.Common.Models;
using System.Threading.Tasks;

namespace GestionEmpresarial.Application.Common.Interfaces
{
    public interface IIdentityService
    {
        Task<Result<AuthenticationResponse>> AuthenticateAsync(string username, string password, string ipAddress);
        Task<Result<AuthenticationResponse>> RefreshTokenAsync(string token, string ipAddress);
        Task<Result> RevokeTokenAsync(string token, string ipAddress);
        Task<Result<string>> RegisterUserAsync(string username, string email, string password);
    }
}
