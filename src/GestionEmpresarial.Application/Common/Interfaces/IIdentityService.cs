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
    }
}
