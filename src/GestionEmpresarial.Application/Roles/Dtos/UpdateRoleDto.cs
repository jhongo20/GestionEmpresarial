using System;

namespace GestionEmpresarial.Application.Roles.Dtos
{
    public class UpdateRoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
