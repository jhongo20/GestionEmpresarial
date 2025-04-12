using GestionEmpresarial.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GestionEmpresarial.Application.Users.Dtos
{
    public class CreateLdapUserDto
    {
        [Required(ErrorMessage = "El nombre de usuario es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre de usuario debe tener entre 3 y 50 caracteres", MinimumLength = 3)]
        public string Username { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres", MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [StringLength(50, ErrorMessage = "El apellido debe tener entre 2 y 50 caracteres", MinimumLength = 2)]
        public string LastName { get; set; }

        [Phone(ErrorMessage = "El formato del número de teléfono no es válido")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "El estado del usuario es obligatorio")]
        public UserStatus Status { get; set; }

        [Required(ErrorMessage = "El tipo de usuario es obligatorio")]
        public UserType UserType { get; set; }

        [Required(ErrorMessage = "Se requiere al menos un rol")]
        public List<Guid> RoleIds { get; set; }
    }
}
