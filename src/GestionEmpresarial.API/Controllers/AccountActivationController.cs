using GestionEmpresarial.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GestionEmpresarial.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountActivationController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuditService _auditService;

        public AccountActivationController(
            IUserService userService,
            IAuditService auditService)
        {
            _userService = userService;
            _auditService = auditService;
        }

        [HttpPost("activate-with-token")]
        [AllowAnonymous]
        public async Task<IActionResult> ActivateWithToken([FromBody] ActivateWithTokenRequest request)
        {
            if (string.IsNullOrEmpty(request.Token))
            {
                return BadRequest("El token de activación es requerido.");
            }

            var result = await _userService.ActivateAccountWithTokenAsync(request.Token);

            if (result.Succeeded)
            {
                await _auditService.LogActionAsync(
                    "Anonymous",
                    "Anonymous",
                    "AccountActivation",
                    "Users",
                    "N/A",
                    null,
                    null,
                    "Cuenta activada con token",
                    HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Request.Headers["User-Agent"].ToString());

                return Ok(new { message = "Cuenta activada correctamente." });
            }

            return BadRequest(result.Errors);
        }

        [HttpPost("activate-with-code")]
        [AllowAnonymous]
        public async Task<IActionResult> ActivateWithCode([FromBody] ActivateWithCodeRequest request)
        {
            if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Code))
            {
                return BadRequest("El correo electrónico y el código de activación son requeridos.");
            }

            var result = await _userService.ActivateAccountWithCodeAsync(request.Email, request.Code);
            
            if (result.Succeeded)
            {
                await _auditService.LogActionAsync(
                    "Anonymous",
                    "Anonymous",
                    "AccountActivation",
                    "Users",
                    "N/A",
                    null,
                    null,
                    "Cuenta activada con código",
                    HttpContext.Connection.RemoteIpAddress?.ToString(),
                    Request.Headers["User-Agent"].ToString());

                return Ok(new { message = "Cuenta activada correctamente." });
            }

            return BadRequest(result.Errors);
        }
    }

    public class ActivateWithTokenRequest
    {
        public string Token { get; set; }
    }

    public class ActivateWithCodeRequest
    {
        public string Email { get; set; }
        public string Code { get; set; }
    }
}
