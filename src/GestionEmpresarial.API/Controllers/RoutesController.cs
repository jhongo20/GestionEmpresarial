using GestionEmpresarial.Application.Common.Interfaces;
using GestionEmpresarial.Application.Common.Models;
using GestionEmpresarial.Application.Routes.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GestionEmpresarial.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoutesController : ControllerBase
    {
        private readonly IRouteService _routeService;

        public RoutesController(IRouteService routeService)
        {
            _routeService = routeService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllRoutes()
        {
            var result = await _routeService.GetAllRoutesAsync();

            if (result.Succeeded)
                return Ok(result.Data);

            return BadRequest(result.Errors.FirstOrDefault());
        }

        [HttpGet("paged")]
        [Authorize]
        public async Task<IActionResult> GetRoutesPaged([FromQuery] PaginationParams paginationParams)
        {
            var result = await _routeService.GetRoutesPagedAsync(paginationParams);

            if (result.Succeeded)
                return Ok(result.Data);

            return BadRequest(result.Errors.FirstOrDefault());
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetRouteById(Guid id)
        {
            var result = await _routeService.GetRouteByIdAsync(id);

            if (result.Succeeded)
                return Ok(result.Data);

            return NotFound(result.Errors.FirstOrDefault());
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateRoute([FromBody] CreateRouteDto createRouteDto)
        {
            var result = await _routeService.CreateRouteAsync(createRouteDto);

            if (result.Succeeded)
                return CreatedAtAction(nameof(GetRouteById), new { id = result.Data.Id }, result.Data);

            return BadRequest(result.Errors.FirstOrDefault());
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateRoute(Guid id, [FromBody] UpdateRouteDto updateRouteDto)
        {
            if (id != updateRouteDto.Id)
                return BadRequest("El ID en la URL no coincide con el ID en el cuerpo de la solicitud.");

            var result = await _routeService.UpdateRouteAsync(updateRouteDto);

            if (result.Succeeded)
                return Ok(result.Data);

            return BadRequest(result.Errors.FirstOrDefault());
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteRoute(Guid id)
        {
            var result = await _routeService.DeleteRouteAsync(id);

            if (result.Succeeded)
                return NoContent();

            return BadRequest(result.Errors.FirstOrDefault());
        }

        [HttpPut("{id}/toggle-status")]
        [Authorize]
        public async Task<IActionResult> ToggleRouteStatus(Guid id)
        {
            var result = await _routeService.ToggleRouteStatusAsync(id);

            if (result.Succeeded)
                return Ok(new { Message = "Estado de la ruta actualizado correctamente" });

            return BadRequest(result.Errors.FirstOrDefault());
        }

        [HttpGet("by-module/{moduleId}")]
        [Authorize]
        public async Task<IActionResult> GetRoutesByModule(Guid moduleId)
        {
            var result = await _routeService.GetRoutesByModuleAsync(moduleId);

            if (result.Succeeded)
                return Ok(result.Data);

            return BadRequest(result.Errors.FirstOrDefault());
        }

        [HttpGet("by-role/{roleId}")]
        [Authorize]
        public async Task<IActionResult> GetRoutesByRole(Guid roleId)
        {
            var result = await _routeService.GetRoutesByRoleAsync(roleId);

            if (result.Succeeded)
                return Ok(result.Data);

            return BadRequest(result.Errors.FirstOrDefault());
        }
    }
}
