using GestionEmpresarial.Application.Common.Models;
using GestionEmpresarial.Application.Permissions.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestionEmpresarial.Application.Common.Interfaces
{
    public interface IPermissionService
    {
        Task<Result<List<PermissionDto>>> GetAllPermissionsAsync();
        Task<Result<PaginatedList<PermissionDto>>> GetPermissionsPagedAsync(PaginationParams paginationParams);
        Task<Result<PermissionDto>> GetPermissionByIdAsync(Guid id);
        Task<Result<PermissionDto>> CreatePermissionAsync(CreatePermissionDto createPermissionDto);
        Task<Result<PermissionDto>> UpdatePermissionAsync(UpdatePermissionDto updatePermissionDto);
        Task<Result> DeletePermissionAsync(Guid id);
    }
}
