using System;
using System.ComponentModel.DataAnnotations;

namespace GestionEmpresarial.Application.Roles.Dtos
{
    public class AssignRouteToRoleDto
    {
        [Required(ErrorMessage = "El ID del rol es obligatorio")]
        public Guid RoleId { get; set; }

        [Required(ErrorMessage = "El ID de la ruta es obligatorio")]
        public Guid RouteId { get; set; }
    }
}
