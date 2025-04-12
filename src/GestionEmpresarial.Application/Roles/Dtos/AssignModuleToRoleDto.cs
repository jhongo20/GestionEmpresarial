using System;
using System.ComponentModel.DataAnnotations;

namespace GestionEmpresarial.Application.Roles.Dtos
{
    public class AssignModuleToRoleDto
    {
        [Required(ErrorMessage = "El ID del rol es obligatorio")]
        public Guid RoleId { get; set; }

        [Required(ErrorMessage = "El ID del módulo es obligatorio")]
        public Guid ModuleId { get; set; }
    }
}
