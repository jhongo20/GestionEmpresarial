using System;
using System.ComponentModel.DataAnnotations;

namespace GestionEmpresarial.Application.Routes.Dtos
{
    public class UpdateRouteDto
    {
        [Required(ErrorMessage = "El ID es obligatorio")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres", MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "La ruta es obligatoria")]
        [StringLength(200, ErrorMessage = "La ruta no puede exceder los 200 caracteres")]
        public string Path { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "El icono no puede exceder los 50 caracteres")]
        public string Icon { get; set; } = string.Empty;

        [Required(ErrorMessage = "El orden es obligatorio")]
        public int Order { get; set; }

        public bool IsActive { get; set; }

        [Required(ErrorMessage = "El ID del módulo es obligatorio")]
        public Guid ModuleId { get; set; }
    }
}
