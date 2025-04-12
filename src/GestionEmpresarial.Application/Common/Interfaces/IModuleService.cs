using GestionEmpresarial.Application.Common.Models;
using GestionEmpresarial.Application.Modules.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestionEmpresarial.Application.Common.Interfaces
{
    public interface IModuleService
    {
        Task<Result<List<ModuleDto>>> GetAllModulesAsync();
        Task<Result<PaginatedList<ModuleDto>>> GetModulesPagedAsync(PaginationParams paginationParams);
        Task<Result<ModuleDto>> GetModuleByIdAsync(Guid id);
        Task<Result<ModuleDto>> CreateModuleAsync(CreateModuleDto createModuleDto);
        Task<Result<ModuleDto>> UpdateModuleAsync(UpdateModuleDto updateModuleDto);
        Task<Result<bool>> DeleteModuleAsync(Guid id);
        Task<Result<bool>> ToggleModuleStatusAsync(Guid id);
        Task<Result<List<ModuleDto>>> GetModulesByRoleAsync(Guid roleId);
    }
}
