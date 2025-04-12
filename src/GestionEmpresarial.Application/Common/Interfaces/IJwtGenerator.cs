using GestionEmpresarial.Domain.Entities;
using System;
using System.Collections.Generic;

namespace GestionEmpresarial.Application.Common.Interfaces
{
    public interface IJwtGenerator
    {
        (string Token, DateTime Expiration) GenerateJwtToken(string userId, string username, string email, IList<string> roles);
        RefreshToken GenerateRefreshToken(string ipAddress);
    }
}
