using GestionEmpresarial.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;

namespace GestionEmpresarial.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IIdentityService _identityService;

        public AuthController(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _identityService.AuthenticateAsync(
                request.Username,
                request.Password);

            if (result == null)
            {
                return Unauthorized(new { Error = "Usuario o contraseña incorrectos" });
            }

            return Ok(result);
        }

        [HttpGet("test")]
        [AllowAnonymous]
        public IActionResult Test()
        {
            return Ok(new { Message = "API de autenticación funcionando correctamente" });
        }

        [HttpGet("protected")]
        [Authorize]
        public IActionResult Protected()
        {
            return Ok(new { 
                Message = "Este es un endpoint protegido. Si puedes ver esto, estás autenticado correctamente.",
                User = User.Identity?.Name,
                Claims = User.Claims.Select(c => new { c.Type, c.Value })
            });
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
