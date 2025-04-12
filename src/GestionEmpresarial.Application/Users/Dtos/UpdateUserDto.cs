using GestionEmpresarial.Domain.Enums;
using System;
using System.Collections.Generic;

namespace GestionEmpresarial.Application.Users.Dtos
{
    public class UpdateUserDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public UserStatus Status { get; set; }
        public UserType UserType { get; set; }
        public List<Guid> RoleIds { get; set; } = new List<Guid>();
    }
}
