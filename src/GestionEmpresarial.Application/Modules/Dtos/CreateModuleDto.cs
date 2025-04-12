using System.ComponentModel.DataAnnotations;

namespace GestionEmpresarial.Application.Modules.Dtos
{
    public class CreateModuleDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres", MinimumLength = 3)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres")]
        public string Description { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "El icono no puede exceder los 50 caracteres")]
        public string Icon { get; set; } = string.Empty;

        [Required(ErrorMessage = "El orden es obligatorio")]
        public int Order { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
