using System;

namespace GestionEmpresarial.Application.Permissions.Dtos
{
    public class UpdatePermissionDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
