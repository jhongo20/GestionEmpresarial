using GestionEmpresarial.Application.Common.Interfaces;
using GestionEmpresarial.Application.Users.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace GestionEmpresarial.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IUserService userService, ILogger<AccountController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Activa una cuenta de usuario utilizando el token de activación
        /// </summary>
        /// <param name="activateAccountDto">DTO con el token de activación</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPost("activate")]
        [AllowAnonymous]
        public async Task<IActionResult> ActivateAccount([FromBody] ActivateAccountDto activateAccountDto)
        {
            var result = await _userService.ActivateAccountAsync(activateAccountDto);
            if (result.Succeeded)
            {
                return Ok(new { message = "Cuenta activada correctamente" });
            }
            return BadRequest(new { message = result.Errors.FirstOrDefault() });
        }

        /// <summary>
        /// Reenvía el correo de activación a un usuario
        /// </summary>
        /// <param name="email">Correo electrónico del usuario</param>
        /// <returns>Resultado de la operación</returns>
        [HttpPost("resend-activation")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendActivation([FromBody] ResendActivationDto resendActivationDto)
        {
            var result = await _userService.ResendActivationEmailAsync(resendActivationDto.Email);
            if (result.Succeeded)
            {
                return Ok(new { message = "Correo de activación reenviado correctamente" });
            }
            return BadRequest(new { message = result.Errors.FirstOrDefault() });
        }

        /// <summary>
        /// Genera un nuevo token de activación para un usuario (solo para administradores)
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <returns>Token de activación</returns>
        [HttpPost("generate-activation-token/{userId}")]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> GenerateActivationToken(Guid userId)
        {
            var result = await _userService.GenerateActivationTokenAsync(userId);
            if (result.Succeeded)
            {
                return Ok(new { token = result.Data });
            }
            return BadRequest(new { message = result.Errors.FirstOrDefault() });
        }
    }

    public class ResendActivationDto
    {
        [Required(ErrorMessage = "El correo electrónico es requerido")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        public string Email { get; set; } = string.Empty;
    }
}
