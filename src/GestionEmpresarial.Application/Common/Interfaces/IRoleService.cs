using GestionEmpresarial.Application.Common.Models;
using GestionEmpresarial.Application.Roles.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestionEmpresarial.Application.Common.Interfaces
{
    public interface IRoleService
    {
        Task<Result<List<RoleDto>>> GetAllRolesAsync();
        Task<Result<PaginatedList<RoleDto>>> GetRolesPagedAsync(PaginationParams paginationParams);
        Task<Result<RoleDto>> GetRoleByIdAsync(Guid id);
        Task<Result<RoleDto>> CreateRoleAsync(CreateRoleDto createRoleDto);
        Task<Result<RoleDto>> UpdateRoleAsync(UpdateRoleDto updateRoleDto);
        Task<Result<bool>> DeleteRoleAsync(Guid id);
        
        // Nuevos métodos para gestionar módulos y rutas
        Task<Result<bool>> AssignModuleToRoleAsync(AssignModuleToRoleDto assignModuleToRoleDto);
        Task<Result<bool>> RemoveModuleFromRoleAsync(Guid roleId, Guid moduleId);
        Task<Result<bool>> AssignRouteToRoleAsync(AssignRouteToRoleDto assignRouteToRoleDto);
        Task<Result<bool>> RemoveRouteFromRoleAsync(Guid roleId, Guid routeId);
    }
}
