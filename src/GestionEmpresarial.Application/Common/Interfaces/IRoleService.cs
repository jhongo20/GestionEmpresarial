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
        Task<Result> DeleteRoleAsync(Guid id);
    }
}
