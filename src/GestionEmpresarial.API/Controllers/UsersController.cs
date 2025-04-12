using GestionEmpresarial.Application.Common.Interfaces;
using GestionEmpresarial.Application.Common.Models;
using GestionEmpresarial.Application.Users.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GestionEmpresarial.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _userService.GetAllUsersAsync();

            if (result.Succeeded)
                return Ok(result.Data);

            return BadRequest(result.Errors.FirstOrDefault());
        }

        [HttpGet("paged")]
        [Authorize]
        public async Task<IActionResult> GetUsersPaged([FromQuery] PaginationParams paginationParams)
        {
            var result = await _userService.GetUsersPagedAsync(paginationParams);

            if (result.Succeeded)
                return Ok(result.Data);

            return BadRequest(result.Errors.FirstOrDefault());
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var result = await _userService.GetUserByIdAsync(id);

            if (result.Succeeded)
                return Ok(result.Data);

            return NotFound(result.Errors.FirstOrDefault());
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto createUserDto)
        {
            var result = await _userService.CreateUserAsync(createUserDto);

            if (result.Succeeded)
                return CreatedAtAction(nameof(GetUserById), new { id = result.Data.Id }, result.Data);

            return BadRequest(result.Errors.FirstOrDefault());
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserDto updateUserDto)
        {
            if (id != updateUserDto.Id)
                return BadRequest("El ID en la URL no coincide con el ID en el cuerpo de la solicitud.");

            var result = await _userService.UpdateUserAsync(updateUserDto);

            if (result.Succeeded)
                return Ok(result.Data);

            return BadRequest(result.Errors.FirstOrDefault());
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var result = await _userService.DeleteUserAsync(id);

            if (result.Succeeded)
                return NoContent();

            return BadRequest(result.Errors.FirstOrDefault());
        }

        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var result = await _userService.ChangePasswordAsync(changePasswordDto);

            if (result.Succeeded)
                return Ok();

            return BadRequest(result.Errors.FirstOrDefault());
        }

        [HttpPost("ldap")]
        [Authorize]
        public async Task<IActionResult> CreateLdapUser([FromBody] CreateLdapUserDto createLdapUserDto)
        {
            var result = await _userService.CreateLdapUserAsync(createLdapUserDto);

            if (result.Succeeded)
                return CreatedAtAction(nameof(GetUserById), new { id = result.Data.Id }, result.Data);

            return BadRequest(result.Errors.FirstOrDefault());
        }

        [HttpGet("by-role/{roleId}")]
        [Authorize]
        public async Task<IActionResult> GetUsersByRole(Guid roleId)
        {
            var result = await _userService.GetUsersByRoleAsync(roleId);

            if (result.Succeeded)
                return Ok(result.Data);

            return BadRequest(result.Errors.FirstOrDefault());
        }

        [HttpGet("by-role/{roleId}/paged")]
        [Authorize]
        public async Task<IActionResult> GetUsersByRolePaged(Guid roleId, [FromQuery] PaginationParams paginationParams)
        {
            var result = await _userService.GetUsersByRolePagedAsync(roleId, paginationParams);

            if (result.Succeeded)
                return Ok(result.Data);

            return BadRequest(result.Errors.FirstOrDefault());
        }
    }
}
