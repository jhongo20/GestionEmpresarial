using GestionEmpresarial.Application.Common.Models;
using GestionEmpresarial.Application.Users.Dtos;
using System.Threading.Tasks;

namespace GestionEmpresarial.Application.Common.Interfaces
{
    public interface IIdentityService
    {
        Task<AuthenticationResponse> AuthenticateAsync(string username, string password);
        
        Task<UserDto> GetUserByUsernameAsync(string username);
        
        string GenerateTokenForUser(UserDto user);

        Task<(bool Succeeded, string UserId, string UserName, string[] Errors)> ActivateUserAsync(string token);

        Task<(bool Succeeded, string UserId, string UserName, string[] Errors)> CreateUserAsync(string userName, string email, string password);

        Task<(bool Succeeded, string[] Errors)> DeleteUserAsync(string userId);

        Task<(bool Succeeded, string Token, string[] Errors)> GetActivationTokenByEmailAsync(string email);
    }
}
