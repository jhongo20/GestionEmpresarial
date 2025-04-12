using System;

namespace GestionEmpresarial.Application.Common.Interfaces
{
    public interface ICurrentUserService
    {
        string? UserId { get; }
        string? Username { get; }
        string? IpAddress { get; }
        bool IsAuthenticated { get; }
    }
}
