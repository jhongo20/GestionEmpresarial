using GestionEmpresarial.Application.Common.Interfaces;
using GestionEmpresarial.Application.Common.Models;
using GestionEmpresarial.Application.Routes.Dtos;
using GestionEmpresarial.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace GestionEmpresarial.Infrastructure.Services
{
    public class RouteService : IRouteService
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTime _dateTime;
        private readonly ILogger<RouteService> _logger;

        public RouteService(
            IApplicationDbContext context,
            ICurrentUserService currentUserService,
            IDateTime dateTime,
            ILogger<RouteService> logger)
        {
            _context = context;
            _currentUserService = currentUserService;
            _dateTime = dateTime;
            _logger = logger;
        }

        public async Task<Result<List<RouteDto>>> GetAllRoutesAsync()
        {
            try
            {
                var routes = await _context.Routes
                    .Where(r => !r.IsDeleted)
                    .Include(r => r.Module)
                    .OrderBy(r => r.Module.Order)
                    .ThenBy(r => r.Order)
                    .Select(r => new RouteDto
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Path = r.Path,
                        Icon = r.Icon,
                        Order = r.Order,
                        IsActive = r.IsActive,
                        ModuleId = r.ModuleId,
                        ModuleName = r.Module.Name,
                        CreatedAt = r.CreatedAt,
                        CreatedBy = r.CreatedBy,
                        UpdatedAt = r.UpdatedAt,
                        UpdatedBy = r.UpdatedBy
                    })
                    .ToListAsync();

                return Result<List<RouteDto>>.Success(routes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todas las rutas");
                return Result<List<RouteDto>>.Failure($"Error al obtener todas las rutas: {ex.Message}");
            }
        }

        public async Task<Result<PaginatedList<RouteDto>>> GetRoutesPagedAsync(PaginationParams paginationParams)
        {
            try
            {
                var query = _context.Routes
                    .Where(r => !r.IsDeleted)
                    .Include(r => r.Module)
                    .AsQueryable();

                // Aplicar filtrado y ordenación
                if (!string.IsNullOrEmpty(paginationParams.SortBy))
                {
                    var sortDirection = paginationParams.SortDirection?.ToLower() == "desc" ? "descending" : "ascending";
                    var sortProperty = paginationParams.SortBy;
                    
                    // Asegurarse de que la propiedad existe
                    if (typeof(Route).GetProperty(sortProperty) != null)
                    {
                        query = query.OrderBy($"{sortProperty} {sortDirection}");
                    }
                    else
                    {
                        query = query.OrderBy(r => r.Module.Order).ThenBy(r => r.Order);
                    }
                }
                else
                {
                    query = query.OrderBy(r => r.Module.Order).ThenBy(r => r.Order);
                }

                var paginatedRoutes = await PaginatedList<RouteDto>.CreateAsync(
                    query.Select(r => new RouteDto
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Path = r.Path,
                        Icon = r.Icon,
                        Order = r.Order,
                        IsActive = r.IsActive,
                        ModuleId = r.ModuleId,
                        ModuleName = r.Module.Name,
                        CreatedAt = r.CreatedAt,
                        CreatedBy = r.CreatedBy,
                        UpdatedAt = r.UpdatedAt,
                        UpdatedBy = r.UpdatedBy
                    }),
                    paginationParams.PageNumber,
                    paginationParams.PageSize);

                return Result<PaginatedList<RouteDto>>.Success(paginatedRoutes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener rutas paginadas");
                return Result<PaginatedList<RouteDto>>.Failure($"Error al obtener rutas paginadas: {ex.Message}");
            }
        }

        public async Task<Result<RouteDto>> GetRouteByIdAsync(Guid id)
        {
            try
            {
                var route = await _context.Routes
                    .Where(r => r.Id == id && !r.IsDeleted)
                    .Include(r => r.Module)
                    .Select(r => new RouteDto
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Path = r.Path,
                        Icon = r.Icon,
                        Order = r.Order,
                        IsActive = r.IsActive,
                        ModuleId = r.ModuleId,
                        ModuleName = r.Module.Name,
                        CreatedAt = r.CreatedAt,
                        CreatedBy = r.CreatedBy,
                        UpdatedAt = r.UpdatedAt,
                        UpdatedBy = r.UpdatedBy
                    })
                    .FirstOrDefaultAsync();

                if (route == null)
                {
                    return Result<RouteDto>.Failure("La ruta especificada no existe.");
                }

                return Result<RouteDto>.Success(route);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener la ruta con ID {id}");
                return Result<RouteDto>.Failure($"Error al obtener la ruta: {ex.Message}");
            }
        }

        public async Task<Result<RouteDto>> CreateRouteAsync(CreateRouteDto createRouteDto)
        {
            try
            {
                // Verificar si el módulo existe
                var moduleExists = await _context.Modules.AnyAsync(m => m.Id == createRouteDto.ModuleId && !m.IsDeleted);
                if (!moduleExists)
                {
                    return Result<RouteDto>.Failure("El módulo especificado no existe.");
                }

                // Verificar si ya existe una ruta con el mismo nombre en el mismo módulo
                if (await _context.Routes.AnyAsync(r => r.Name == createRouteDto.Name && r.ModuleId == createRouteDto.ModuleId && !r.IsDeleted))
                {
                    return Result<RouteDto>.Failure("Ya existe una ruta con este nombre en el módulo seleccionado.");
                }

                // Verificar si ya existe una ruta con la misma ruta (path)
                if (await _context.Routes.AnyAsync(r => r.Path == createRouteDto.Path && !r.IsDeleted))
                {
                    return Result<RouteDto>.Failure("Ya existe una ruta con esta URL.");
                }

                // Crear la ruta
                var route = new Route
                {
                    Id = Guid.NewGuid(),
                    Name = createRouteDto.Name,
                    Path = createRouteDto.Path,
                    Icon = createRouteDto.Icon,
                    Order = createRouteDto.Order,
                    IsActive = createRouteDto.IsActive,
                    ModuleId = createRouteDto.ModuleId,
                    IsDeleted = false,
                    CreatedBy = _currentUserService.UserId ?? "System",
                    CreatedAt = _dateTime.Now,
                    UpdatedBy = _currentUserService.UserId ?? "System",
                    UpdatedAt = _dateTime.Now
                };

                await _context.Routes.AddAsync(route);
                await _context.SaveChangesAsync(default);

                // Obtener el nombre del módulo
                var moduleName = await _context.Modules
                    .Where(m => m.Id == createRouteDto.ModuleId)
                    .Select(m => m.Name)
                    .FirstOrDefaultAsync();

                // Crear el DTO de respuesta
                var routeDto = new RouteDto
                {
                    Id = route.Id,
                    Name = route.Name,
                    Path = route.Path,
                    Icon = route.Icon,
                    Order = route.Order,
                    IsActive = route.IsActive,
                    ModuleId = route.ModuleId,
                    ModuleName = moduleName,
                    CreatedAt = route.CreatedAt,
                    CreatedBy = route.CreatedBy,
                    UpdatedAt = route.UpdatedAt,
                    UpdatedBy = route.UpdatedBy
                };

                return Result<RouteDto>.Success(routeDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la ruta");
                return Result<RouteDto>.Failure($"Error al crear la ruta: {ex.Message}");
            }
        }

        public async Task<Result<RouteDto>> UpdateRouteAsync(UpdateRouteDto updateRouteDto)
        {
            try
            {
                // Verificar si la ruta existe
                var route = await _context.Routes
                    .Include(r => r.Module)
                    .FirstOrDefaultAsync(r => r.Id == updateRouteDto.Id && !r.IsDeleted);

                if (route == null)
                {
                    return Result<RouteDto>.Failure("La ruta especificada no existe.");
                }

                // Verificar si el módulo existe
                var moduleExists = await _context.Modules.AnyAsync(m => m.Id == updateRouteDto.ModuleId && !m.IsDeleted);
                if (!moduleExists)
                {
                    return Result<RouteDto>.Failure("El módulo especificado no existe.");
                }

                // Verificar si ya existe otra ruta con el mismo nombre en el mismo módulo
                if (await _context.Routes.AnyAsync(r => r.Name == updateRouteDto.Name && r.ModuleId == updateRouteDto.ModuleId && r.Id != updateRouteDto.Id && !r.IsDeleted))
                {
                    return Result<RouteDto>.Failure("Ya existe otra ruta con este nombre en el módulo seleccionado.");
                }

                // Verificar si ya existe otra ruta con la misma ruta (path)
                if (await _context.Routes.AnyAsync(r => r.Path == updateRouteDto.Path && r.Id != updateRouteDto.Id && !r.IsDeleted))
                {
                    return Result<RouteDto>.Failure("Ya existe otra ruta con esta URL.");
                }

                // Actualizar la ruta
                route.Name = updateRouteDto.Name;
                route.Path = updateRouteDto.Path;
                route.Icon = updateRouteDto.Icon;
                route.Order = updateRouteDto.Order;
                route.IsActive = updateRouteDto.IsActive;
                route.ModuleId = updateRouteDto.ModuleId;
                route.UpdatedBy = _currentUserService.UserId ?? "System";
                route.UpdatedAt = _dateTime.Now;

                _context.Routes.Update(route);
                await _context.SaveChangesAsync(default);

                // Obtener el nombre del módulo
                var moduleName = await _context.Modules
                    .Where(m => m.Id == updateRouteDto.ModuleId)
                    .Select(m => m.Name)
                    .FirstOrDefaultAsync();

                // Crear el DTO de respuesta
                var routeDto = new RouteDto
                {
                    Id = route.Id,
                    Name = route.Name,
                    Path = route.Path,
                    Icon = route.Icon,
                    Order = route.Order,
                    IsActive = route.IsActive,
                    ModuleId = route.ModuleId,
                    ModuleName = moduleName,
                    CreatedAt = route.CreatedAt,
                    CreatedBy = route.CreatedBy,
                    UpdatedAt = route.UpdatedAt,
                    UpdatedBy = route.UpdatedBy
                };

                return Result<RouteDto>.Success(routeDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar la ruta con ID {updateRouteDto.Id}");
                return Result<RouteDto>.Failure($"Error al actualizar la ruta: {ex.Message}");
            }
        }

        public async Task<Result<bool>> DeleteRouteAsync(Guid id)
        {
            try
            {
                // Verificar si la ruta existe
                var route = await _context.Routes
                    .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

                if (route == null)
                {
                    return Result<bool>.Failure("La ruta especificada no existe.");
                }

                // Marcar la ruta como eliminada
                route.IsDeleted = true;
                route.UpdatedBy = _currentUserService.UserId ?? "System";
                route.UpdatedAt = _dateTime.Now;

                _context.Routes.Update(route);
                await _context.SaveChangesAsync(default);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar la ruta con ID {id}");
                return Result<bool>.Failure($"Error al eliminar la ruta: {ex.Message}");
            }
        }

        public async Task<Result<bool>> ToggleRouteStatusAsync(Guid id)
        {
            try
            {
                // Verificar si la ruta existe
                var route = await _context.Routes
                    .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

                if (route == null)
                {
                    return Result<bool>.Failure("La ruta especificada no existe.");
                }

                // Cambiar el estado de la ruta
                route.IsActive = !route.IsActive;
                route.UpdatedBy = _currentUserService.UserId ?? "System";
                route.UpdatedAt = _dateTime.Now;

                _context.Routes.Update(route);
                await _context.SaveChangesAsync(default);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al cambiar el estado de la ruta con ID {id}");
                return Result<bool>.Failure($"Error al cambiar el estado de la ruta: {ex.Message}");
            }
        }

        public async Task<Result<List<RouteDto>>> GetRoutesByModuleAsync(Guid moduleId)
        {
            try
            {
                // Verificar si el módulo existe
                var moduleExists = await _context.Modules.AnyAsync(m => m.Id == moduleId && !m.IsDeleted);
                if (!moduleExists)
                {
                    return Result<List<RouteDto>>.Failure($"El módulo con ID {moduleId} no existe.");
                }

                // Obtener las rutas del módulo
                var routes = await _context.Routes
                    .Where(r => r.ModuleId == moduleId && !r.IsDeleted)
                    .Include(r => r.Module)
                    .OrderBy(r => r.Order)
                    .Select(r => new RouteDto
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Path = r.Path,
                        Icon = r.Icon,
                        Order = r.Order,
                        IsActive = r.IsActive,
                        ModuleId = r.ModuleId,
                        ModuleName = r.Module.Name,
                        CreatedAt = r.CreatedAt,
                        CreatedBy = r.CreatedBy,
                        UpdatedAt = r.UpdatedAt,
                        UpdatedBy = r.UpdatedBy
                    })
                    .ToListAsync();

                return Result<List<RouteDto>>.Success(routes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener las rutas del módulo con ID {moduleId}");
                return Result<List<RouteDto>>.Failure($"Error al obtener las rutas del módulo: {ex.Message}");
            }
        }

        public async Task<Result<List<RouteDto>>> GetRoutesByRoleAsync(Guid roleId)
        {
            try
            {
                // Verificar si el rol existe
                var roleExists = await _context.Roles.AnyAsync(r => r.Id == roleId && !r.IsDeleted);
                if (!roleExists)
                {
                    return Result<List<RouteDto>>.Failure($"El rol con ID {roleId} no existe.");
                }

                // Obtener las rutas asignadas al rol
                var routes = await _context.RoleRoutes
                    .Where(rr => rr.RoleId == roleId && !rr.IsDeleted && rr.IsActive)
                    .Include(rr => rr.Route)
                    .ThenInclude(r => r.Module)
                    .Where(rr => !rr.Route.IsDeleted && rr.Route.IsActive)
                    .OrderBy(rr => rr.Route.Module.Order)
                    .ThenBy(rr => rr.Route.Order)
                    .Select(rr => new RouteDto
                    {
                        Id = rr.Route.Id,
                        Name = rr.Route.Name,
                        Path = rr.Route.Path,
                        Icon = rr.Route.Icon,
                        Order = rr.Route.Order,
                        IsActive = rr.Route.IsActive,
                        ModuleId = rr.Route.ModuleId,
                        ModuleName = rr.Route.Module.Name,
                        CreatedAt = rr.Route.CreatedAt,
                        CreatedBy = rr.Route.CreatedBy,
                        UpdatedAt = rr.Route.UpdatedAt,
                        UpdatedBy = rr.Route.UpdatedBy
                    })
                    .ToListAsync();

                return Result<List<RouteDto>>.Success(routes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener las rutas para el rol con ID {roleId}");
                return Result<List<RouteDto>>.Failure($"Error al obtener las rutas para el rol: {ex.Message}");
            }
        }
    }
}
