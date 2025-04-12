using GestionEmpresarial.Domain.Enums;
using System;
using System.Collections.Generic;

namespace GestionEmpresarial.Application.Users.Dtos
{
    public class CreateUserDto
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public UserStatus Status { get; set; } = UserStatus.Active;
        public UserType UserType { get; set; } = UserType.Internal;
        public List<Guid> RoleIds { get; set; } = new List<Guid>();
    }
}
