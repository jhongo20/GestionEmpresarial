using GestionEmpresarial.Application.Common.Interfaces;
using GestionEmpresarial.Application.Common.Models;
using GestionEmpresarial.Application.Roles.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GestionEmpresarial.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllRoles()
        {
            var result = await _roleService.GetAllRolesAsync();

            if (result.Succeeded)
                return Ok(result.Data);

            return BadRequest(result.Errors.FirstOrDefault());
        }

        [HttpGet("paged")]
        [Authorize]
        public async Task<IActionResult> GetRolesPaged([FromQuery] PaginationParams paginationParams)
        {
            var result = await _roleService.GetRolesPagedAsync(paginationParams);

            if (result.Succeeded)
                return Ok(result.Data);

            return BadRequest(result.Errors.FirstOrDefault());
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetRoleById(Guid id)
        {
            var result = await _roleService.GetRoleByIdAsync(id);

            if (result.Succeeded)
                return Ok(result.Data);

            return NotFound(result.Errors.FirstOrDefault());
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateRole([FromBody] CreateRoleDto createRoleDto)
        {
            var result = await _roleService.CreateRoleAsync(createRoleDto);

            if (result.Succeeded)
                return CreatedAtAction(nameof(GetRoleById), new { id = result.Data.Id }, result.Data);

            return BadRequest(result.Errors.FirstOrDefault());
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateRole(Guid id, [FromBody] UpdateRoleDto updateRoleDto)
        {
            if (id != updateRoleDto.Id)
                return BadRequest("El ID en la URL no coincide con el ID en el cuerpo de la solicitud.");

            var result = await _roleService.UpdateRoleAsync(updateRoleDto);

            if (result.Succeeded)
                return Ok(result.Data);

            return BadRequest(result.Errors.FirstOrDefault());
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteRole(Guid id)
        {
            var result = await _roleService.DeleteRoleAsync(id);

            if (result.Succeeded)
                return NoContent();

            return BadRequest(result.Errors.FirstOrDefault());
        }

        [HttpPost("assign-module")]
        [Authorize]
        public async Task<IActionResult> AssignModuleToRole([FromBody] AssignModuleToRoleDto assignModuleToRoleDto)
        {
            var result = await _roleService.AssignModuleToRoleAsync(assignModuleToRoleDto);

            if (result.Succeeded)
                return Ok(new { Message = "Módulo asignado correctamente al rol" });

            return BadRequest(result.Errors.FirstOrDefault());
        }

        [HttpDelete("remove-module/{roleId}/{moduleId}")]
        [Authorize]
        public async Task<IActionResult> RemoveModuleFromRole(Guid roleId, Guid moduleId)
        {
            var result = await _roleService.RemoveModuleFromRoleAsync(roleId, moduleId);

            if (result.Succeeded)
                return Ok(new { Message = "Módulo eliminado correctamente del rol" });

            return BadRequest(result.Errors.FirstOrDefault());
        }

        [HttpPost("assign-route")]
        [Authorize]
        public async Task<IActionResult> AssignRouteToRole([FromBody] AssignRouteToRoleDto assignRouteToRoleDto)
        {
            var result = await _roleService.AssignRouteToRoleAsync(assignRouteToRoleDto);

            if (result.Succeeded)
                return Ok(new { Message = "Ruta asignada correctamente al rol" });

            return BadRequest(result.Errors.FirstOrDefault());
        }

        [HttpDelete("remove-route/{roleId}/{routeId}")]
        [Authorize]
        public async Task<IActionResult> RemoveRouteFromRole(Guid roleId, Guid routeId)
        {
            var result = await _roleService.RemoveRouteFromRoleAsync(roleId, routeId);

            if (result.Succeeded)
                return Ok(new { Message = "Ruta eliminada correctamente del rol" });

            return BadRequest(result.Errors.FirstOrDefault());
        }
    }
}
