using GestionEmpresarial.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GestionEmpresarial.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenusController : ControllerBase
    {
        private readonly IMenuService _menuService;
        private readonly ICurrentUserService _currentUserService;

        public MenusController(
            IMenuService menuService,
            ICurrentUserService currentUserService)
        {
            _menuService = menuService;
            _currentUserService = currentUserService;
        }

        [HttpGet("my-menu")]
        [Authorize]
        public async Task<IActionResult> GetMyMenu()
        {
            // Obtener el ID del usuario actual
            var userId = _currentUserService.UserId;
            
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuario no autenticado");
            }

            var result = await _menuService.GetMenuByUserIdAsync(Guid.Parse(userId));

            if (result.Succeeded)
                return Ok(result.Data);

            return BadRequest(result.Errors.FirstOrDefault());
        }

        [HttpGet("by-role/{roleId}")]
        [Authorize]
        public async Task<IActionResult> GetMenuByRole(Guid roleId)
        {
            var result = await _menuService.GetMenuByRoleIdAsync(roleId);

            if (result.Succeeded)
                return Ok(result.Data);

            return BadRequest(result.Errors.FirstOrDefault());
        }

        [HttpGet("test-menu")]
        [AllowAnonymous]
        public IActionResult GetTestMenu()
        {
            // Menú de ejemplo para pruebas sin autenticación
            var testMenu = new[]
            {
                new {
                    id = Guid.NewGuid(),
                    name = "Dashboard",
                    icon = "dashboard",
                    path = "/dashboard",
                    order = 1,
                    children = new[]
                    {
                        new {
                            id = Guid.NewGuid(),
                            name = "Inicio",
                            path = "/dashboard",
                            icon = "home",
                            order = 1
                        },
                        new {
                            id = Guid.NewGuid(),
                            name = "Estadísticas",
                            path = "/dashboard/stats",
                            icon = "bar_chart",
                            order = 2
                        }
                    }
                },
                new {
                    id = Guid.NewGuid(),
                    name = "Usuarios",
                    icon = "people",
                    path = "/users",
                    order = 2,
                    children = new[]
                    {
                        new {
                            id = Guid.NewGuid(),
                            name = "Lista de Usuarios",
                            path = "/users/list",
                            icon = "list",
                            order = 1
                        },
                        new {
                            id = Guid.NewGuid(),
                            name = "Crear Usuario",
                            path = "/users/create",
                            icon = "add",
                            order = 2
                        }
                    }
                }
            };

            return Ok(testMenu);
        }
    }
}
