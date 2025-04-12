using GestionEmpresarial.Application.Common.Models;
using System.Threading.Tasks;

namespace GestionEmpresarial.Application.Common.Interfaces
{
    public interface IIdentityService
    {
        Task<AuthenticationResponse> AuthenticateAsync(string username, string password);
    }
}
