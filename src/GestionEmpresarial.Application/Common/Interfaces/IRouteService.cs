using GestionEmpresarial.Application.Common.Models;
using GestionEmpresarial.Application.Routes.Dtos;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestionEmpresarial.Application.Common.Interfaces
{
    public interface IRouteService
    {
        Task<Result<List<RouteDto>>> GetAllRoutesAsync();
        Task<Result<PaginatedList<RouteDto>>> GetRoutesPagedAsync(PaginationParams paginationParams);
        Task<Result<RouteDto>> GetRouteByIdAsync(Guid id);
        Task<Result<RouteDto>> CreateRouteAsync(CreateRouteDto createRouteDto);
        Task<Result<RouteDto>> UpdateRouteAsync(UpdateRouteDto updateRouteDto);
        Task<Result<bool>> DeleteRouteAsync(Guid id);
        Task<Result<bool>> ToggleRouteStatusAsync(Guid id);
        Task<Result<List<RouteDto>>> GetRoutesByModuleAsync(Guid moduleId);
        Task<Result<List<RouteDto>>> GetRoutesByRoleAsync(Guid roleId);
    }
}
