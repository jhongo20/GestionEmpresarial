using GestionEmpresarial.Application.Common.Interfaces;
using GestionEmpresarial.Application.Common.Models;
using GestionEmpresarial.Application.Modules.Dtos;
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
    public class ModuleService : IModuleService
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTime _dateTime;
        private readonly ILogger<ModuleService> _logger;

        public ModuleService(
            IApplicationDbContext context,
            ICurrentUserService currentUserService,
            IDateTime dateTime,
            ILogger<ModuleService> logger)
        {
            _context = context;
            _currentUserService = currentUserService;
            _dateTime = dateTime;
            _logger = logger;
        }

        public async Task<Result<List<ModuleDto>>> GetAllModulesAsync()
        {
            try
            {
                var modules = await _context.Modules
                    .Where(m => !m.IsDeleted)
                    .OrderBy(m => m.Order)
                    .Select(m => new ModuleDto
                    {
                        Id = m.Id,
                        Name = m.Name,
                        Description = m.Description,
                        Icon = m.Icon,
                        Order = m.Order,
                        IsActive = m.IsActive,
                        Routes = m.Routes
                            .Where(r => !r.IsDeleted && r.IsActive)
                            .OrderBy(r => r.Order)
                            .Select(r => new Application.Routes.Dtos.RouteDto
                            {
                                Id = r.Id,
                                Name = r.Name,
                                Path = r.Path,
                                Icon = r.Icon,
                                Order = r.Order,
                                IsActive = r.IsActive,
                                ModuleId = r.ModuleId,
                                ModuleName = m.Name,
                                CreatedAt = r.CreatedAt,
                                CreatedBy = r.CreatedBy,
                                UpdatedAt = r.UpdatedAt,
                                UpdatedBy = r.UpdatedBy
                            }).ToList(),
                        CreatedAt = m.CreatedAt,
                        CreatedBy = m.CreatedBy,
                        UpdatedAt = m.UpdatedAt,
                        UpdatedBy = m.UpdatedBy
                    })
                    .ToListAsync();

                return Result<List<ModuleDto>>.Success(modules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los módulos");
                return Result<List<ModuleDto>>.Failure($"Error al obtener todos los módulos: {ex.Message}");
            }
        }

        public async Task<Result<PaginatedList<ModuleDto>>> GetModulesPagedAsync(PaginationParams paginationParams)
        {
            try
            {
                var query = _context.Modules
                    .Where(m => !m.IsDeleted)
                    .AsQueryable();

                // Aplicar filtrado y ordenación
                if (!string.IsNullOrEmpty(paginationParams.SortBy))
                {
                    var sortDirection = paginationParams.SortDirection?.ToLower() == "desc" ? "descending" : "ascending";
                    var sortProperty = paginationParams.SortBy;
                    
                    // Asegurarse de que la propiedad existe
                    if (typeof(Module).GetProperty(sortProperty) != null)
                    {
                        query = query.OrderBy($"{sortProperty} {sortDirection}");
                    }
                    else
                    {
                        query = query.OrderBy(m => m.Order);
                    }
                }
                else
                {
                    query = query.OrderBy(m => m.Order);
                }

                var paginatedModules = await PaginatedList<ModuleDto>.CreateAsync(
                    query.Select(m => new ModuleDto
                    {
                        Id = m.Id,
                        Name = m.Name,
                        Description = m.Description,
                        Icon = m.Icon,
                        Order = m.Order,
                        IsActive = m.IsActive,
                        Routes = m.Routes
                            .Where(r => !r.IsDeleted && r.IsActive)
                            .OrderBy(r => r.Order)
                            .Select(r => new Application.Routes.Dtos.RouteDto
                            {
                                Id = r.Id,
                                Name = r.Name,
                                Path = r.Path,
                                Icon = r.Icon,
                                Order = r.Order,
                                IsActive = r.IsActive,
                                ModuleId = r.ModuleId,
                                ModuleName = m.Name,
                                CreatedAt = r.CreatedAt,
                                CreatedBy = r.CreatedBy,
                                UpdatedAt = r.UpdatedAt,
                                UpdatedBy = r.UpdatedBy
                            }).ToList(),
                        CreatedAt = m.CreatedAt,
                        CreatedBy = m.CreatedBy,
                        UpdatedAt = m.UpdatedAt,
                        UpdatedBy = m.UpdatedBy
                    }),
                    paginationParams.PageNumber,
                    paginationParams.PageSize);

                return Result<PaginatedList<ModuleDto>>.Success(paginatedModules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener módulos paginados");
                return Result<PaginatedList<ModuleDto>>.Failure($"Error al obtener módulos paginados: {ex.Message}");
            }
        }

        public async Task<Result<ModuleDto>> GetModuleByIdAsync(Guid id)
        {
            try
            {
                var module = await _context.Modules
                    .Where(m => m.Id == id && !m.IsDeleted)
                    .Select(m => new ModuleDto
                    {
                        Id = m.Id,
                        Name = m.Name,
                        Description = m.Description,
                        Icon = m.Icon,
                        Order = m.Order,
                        IsActive = m.IsActive,
                        Routes = m.Routes
                            .Where(r => !r.IsDeleted)
                            .OrderBy(r => r.Order)
                            .Select(r => new Application.Routes.Dtos.RouteDto
                            {
                                Id = r.Id,
                                Name = r.Name,
                                Path = r.Path,
                                Icon = r.Icon,
                                Order = r.Order,
                                IsActive = r.IsActive,
                                ModuleId = r.ModuleId,
                                ModuleName = m.Name,
                                CreatedAt = r.CreatedAt,
                                CreatedBy = r.CreatedBy,
                                UpdatedAt = r.UpdatedAt,
                                UpdatedBy = r.UpdatedBy
                            }).ToList(),
                        CreatedAt = m.CreatedAt,
                        CreatedBy = m.CreatedBy,
                        UpdatedAt = m.UpdatedAt,
                        UpdatedBy = m.UpdatedBy
                    })
                    .FirstOrDefaultAsync();

                if (module == null)
                {
                    return Result<ModuleDto>.Failure("El módulo especificado no existe.");
                }

                return Result<ModuleDto>.Success(module);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener el módulo con ID {id}");
                return Result<ModuleDto>.Failure($"Error al obtener el módulo: {ex.Message}");
            }
        }

        public async Task<Result<ModuleDto>> CreateModuleAsync(CreateModuleDto createModuleDto)
        {
            try
            {
                // Verificar si ya existe un módulo con el mismo nombre
                if (await _context.Modules.AnyAsync(m => m.Name == createModuleDto.Name && !m.IsDeleted))
                {
                    return Result<ModuleDto>.Failure("Ya existe un módulo con este nombre.");
                }

                // Crear el módulo
                var module = new Module
                {
                    Id = Guid.NewGuid(),
                    Name = createModuleDto.Name,
                    Description = createModuleDto.Description,
                    Icon = createModuleDto.Icon,
                    Order = createModuleDto.Order,
                    IsActive = createModuleDto.IsActive,
                    IsDeleted = false,
                    CreatedBy = _currentUserService.UserId ?? "System",
                    CreatedAt = _dateTime.Now,
                    UpdatedBy = _currentUserService.UserId ?? "System",
                    UpdatedAt = _dateTime.Now
                };

                await _context.Modules.AddAsync(module);
                await _context.SaveChangesAsync(default);

                // Crear el DTO de respuesta
                var moduleDto = new ModuleDto
                {
                    Id = module.Id,
                    Name = module.Name,
                    Description = module.Description,
                    Icon = module.Icon,
                    Order = module.Order,
                    IsActive = module.IsActive,
                    Routes = new List<Application.Routes.Dtos.RouteDto>(),
                    CreatedAt = module.CreatedAt,
                    CreatedBy = module.CreatedBy,
                    UpdatedAt = module.UpdatedAt,
                    UpdatedBy = module.UpdatedBy
                };

                return Result<ModuleDto>.Success(moduleDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el módulo");
                return Result<ModuleDto>.Failure($"Error al crear el módulo: {ex.Message}");
            }
        }

        public async Task<Result<ModuleDto>> UpdateModuleAsync(UpdateModuleDto updateModuleDto)
        {
            try
            {
                // Verificar si el módulo existe
                var module = await _context.Modules
                    .Include(m => m.Routes)
                    .FirstOrDefaultAsync(m => m.Id == updateModuleDto.Id && !m.IsDeleted);

                if (module == null)
                {
                    return Result<ModuleDto>.Failure("El módulo especificado no existe.");
                }

                // Verificar si ya existe otro módulo con el mismo nombre
                if (await _context.Modules.AnyAsync(m => m.Name == updateModuleDto.Name && m.Id != updateModuleDto.Id && !m.IsDeleted))
                {
                    return Result<ModuleDto>.Failure("Ya existe otro módulo con este nombre.");
                }

                // Actualizar el módulo
                module.Name = updateModuleDto.Name;
                module.Description = updateModuleDto.Description;
                module.Icon = updateModuleDto.Icon;
                module.Order = updateModuleDto.Order;
                module.IsActive = updateModuleDto.IsActive;
                module.UpdatedBy = _currentUserService.UserId ?? "System";
                module.UpdatedAt = _dateTime.Now;

                _context.Modules.Update(module);
                await _context.SaveChangesAsync(default);

                // Crear el DTO de respuesta
                var moduleDto = new ModuleDto
                {
                    Id = module.Id,
                    Name = module.Name,
                    Description = module.Description,
                    Icon = module.Icon,
                    Order = module.Order,
                    IsActive = module.IsActive,
                    Routes = module.Routes
                        .Where(r => !r.IsDeleted)
                        .OrderBy(r => r.Order)
                        .Select(r => new Application.Routes.Dtos.RouteDto
                        {
                            Id = r.Id,
                            Name = r.Name,
                            Path = r.Path,
                            Icon = r.Icon,
                            Order = r.Order,
                            IsActive = r.IsActive,
                            ModuleId = r.ModuleId,
                            ModuleName = module.Name,
                            CreatedAt = r.CreatedAt,
                            CreatedBy = r.CreatedBy,
                            UpdatedAt = r.UpdatedAt,
                            UpdatedBy = r.UpdatedBy
                        }).ToList(),
                    CreatedAt = module.CreatedAt,
                    CreatedBy = module.CreatedBy,
                    UpdatedAt = module.UpdatedAt,
                    UpdatedBy = module.UpdatedBy
                };

                return Result<ModuleDto>.Success(moduleDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al actualizar el módulo con ID {updateModuleDto.Id}");
                return Result<ModuleDto>.Failure($"Error al actualizar el módulo: {ex.Message}");
            }
        }

        public async Task<Result<bool>> DeleteModuleAsync(Guid id)
        {
            try
            {
                // Verificar si el módulo existe
                var module = await _context.Modules
                    .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);

                if (module == null)
                {
                    return Result<bool>.Failure("El módulo especificado no existe.");
                }

                // Marcar el módulo como eliminado
                module.IsDeleted = true;
                module.UpdatedBy = _currentUserService.UserId ?? "System";
                module.UpdatedAt = _dateTime.Now;

                _context.Modules.Update(module);
                await _context.SaveChangesAsync(default);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el módulo con ID {id}");
                return Result<bool>.Failure($"Error al eliminar el módulo: {ex.Message}");
            }
        }

        public async Task<Result<bool>> ToggleModuleStatusAsync(Guid id)
        {
            try
            {
                // Verificar si el módulo existe
                var module = await _context.Modules
                    .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted);

                if (module == null)
                {
                    return Result<bool>.Failure("El módulo especificado no existe.");
                }

                // Cambiar el estado del módulo
                module.IsActive = !module.IsActive;
                module.UpdatedBy = _currentUserService.UserId ?? "System";
                module.UpdatedAt = _dateTime.Now;

                _context.Modules.Update(module);
                await _context.SaveChangesAsync(default);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al cambiar el estado del módulo con ID {id}");
                return Result<bool>.Failure($"Error al cambiar el estado del módulo: {ex.Message}");
            }
        }

        public async Task<Result<List<ModuleDto>>> GetModulesByRoleAsync(Guid roleId)
        {
            try
            {
                // Verificar si el rol existe
                var roleExists = await _context.Roles.AnyAsync(r => r.Id == roleId && !r.IsDeleted);
                if (!roleExists)
                {
                    return Result<List<ModuleDto>>.Failure($"El rol con ID {roleId} no existe.");
                }

                // Obtener los módulos asignados al rol
                var modules = await _context.RoleModules
                    .Where(rm => rm.RoleId == roleId && !rm.IsDeleted && rm.IsActive)
                    .Include(rm => rm.Module)
                    .ThenInclude(m => m.Routes)
                    .Where(rm => !rm.Module.IsDeleted && rm.Module.IsActive)
                    .OrderBy(rm => rm.Module.Order)
                    .Select(rm => new ModuleDto
                    {
                        Id = rm.Module.Id,
                        Name = rm.Module.Name,
                        Description = rm.Module.Description,
                        Icon = rm.Module.Icon,
                        Order = rm.Module.Order,
                        IsActive = rm.Module.IsActive,
                        Routes = rm.Module.Routes
                            .Where(r => !r.IsDeleted && r.IsActive)
                            .Where(r => r.RoleRoutes.Any(rr => rr.RoleId == roleId && !rr.IsDeleted && rr.IsActive))
                            .OrderBy(r => r.Order)
                            .Select(r => new Application.Routes.Dtos.RouteDto
                            {
                                Id = r.Id,
                                Name = r.Name,
                                Path = r.Path,
                                Icon = r.Icon,
                                Order = r.Order,
                                IsActive = r.IsActive,
                                ModuleId = r.ModuleId,
                                ModuleName = rm.Module.Name,
                                CreatedAt = r.CreatedAt,
                                CreatedBy = r.CreatedBy,
                                UpdatedAt = r.UpdatedAt,
                                UpdatedBy = r.UpdatedBy
                            }).ToList(),
                        CreatedAt = rm.Module.CreatedAt,
                        CreatedBy = rm.Module.CreatedBy,
                        UpdatedAt = rm.Module.UpdatedAt,
                        UpdatedBy = rm.Module.UpdatedBy
                    })
                    .ToListAsync();

                return Result<List<ModuleDto>>.Success(modules);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener los módulos para el rol con ID {roleId}");
                return Result<List<ModuleDto>>.Failure($"Error al obtener los módulos para el rol: {ex.Message}");
            }
        }
    }
}
