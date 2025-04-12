using System;
using System.ComponentModel.DataAnnotations;

namespace GestionEmpresarial.Application.Users.Dtos
{
    public class ActivateAccountDto
    {
        [Required(ErrorMessage = "El token de activación es requerido")]
        public string Token { get; set; }
    }
}
