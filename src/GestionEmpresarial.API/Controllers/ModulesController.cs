using GestionEmpresarial.Application.Common.Interfaces;
using GestionEmpresarial.Application.Common.Models;
using GestionEmpresarial.Application.Modules.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GestionEmpresarial.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModulesController : ControllerBase
    {
        private readonly IModuleService _moduleService;

        public ModulesController(IModuleService moduleService)
        {
            _moduleService = moduleService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllModules()
        {
            var result = await _moduleService.GetAllModulesAsync();

            if (result.Succeeded)
                return Ok(result.Data);

            return BadRequest(result.Errors.FirstOrDefault());
        }

        [HttpGet("paged")]
        [Authorize]
        public async Task<IActionResult> GetModulesPaged([FromQuery] PaginationParams paginationParams)
        {
            var result = await _moduleService.GetModulesPagedAsync(paginationParams);

            if (result.Succeeded)
                return Ok(result.Data);

            return BadRequest(result.Errors.FirstOrDefault());
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetModuleById(Guid id)
        {
            var result = await _moduleService.GetModuleByIdAsync(id);

            if (result.Succeeded)
                return Ok(result.Data);

            return NotFound(result.Errors.FirstOrDefault());
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateModule([FromBody] CreateModuleDto createModuleDto)
        {
            var result = await _moduleService.CreateModuleAsync(createModuleDto);

            if (result.Succeeded)
                return CreatedAtAction(nameof(GetModuleById), new { id = result.Data.Id }, result.Data);

            return BadRequest(result.Errors.FirstOrDefault());
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateModule(Guid id, [FromBody] UpdateModuleDto updateModuleDto)
        {
            if (id != updateModuleDto.Id)
                return BadRequest("El ID en la URL no coincide con el ID en el cuerpo de la solicitud.");

            var result = await _moduleService.UpdateModuleAsync(updateModuleDto);

            if (result.Succeeded)
                return Ok(result.Data);

            return BadRequest(result.Errors.FirstOrDefault());
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteModule(Guid id)
        {
            var result = await _moduleService.DeleteModuleAsync(id);

            if (result.Succeeded)
                return NoContent();

            return BadRequest(result.Errors.FirstOrDefault());
        }

        [HttpPut("{id}/toggle-status")]
        [Authorize]
        public async Task<IActionResult> ToggleModuleStatus(Guid id)
        {
            var result = await _moduleService.ToggleModuleStatusAsync(id);

            if (result.Succeeded)
                return Ok(new { Message = "Estado del módulo actualizado correctamente" });

            return BadRequest(result.Errors.FirstOrDefault());
        }

        [HttpGet("by-role/{roleId}")]
        [Authorize]
        public async Task<IActionResult> GetModulesByRole(Guid roleId)
        {
            var result = await _moduleService.GetModulesByRoleAsync(roleId);

            if (result.Succeeded)
                return Ok(result.Data);

            return BadRequest(result.Errors.FirstOrDefault());
        }
    }
}
