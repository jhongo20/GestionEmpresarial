using GestionEmpresarial.Application.Common.Interfaces;
using GestionEmpresarial.Application.Common.Models;
using GestionEmpresarial.Application.Roles.Dtos;
using GestionEmpresarial.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;

namespace GestionEmpresarial.Infrastructure.Services
{
    public class RoleService : IRoleService
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTime _dateTime;

        public RoleService(
            IApplicationDbContext context,
            ICurrentUserService currentUserService,
            IDateTime dateTime)
        {
            _context = context;
            _currentUserService = currentUserService;
            _dateTime = dateTime;
        }

        public async Task<Result<PaginatedList<RoleDto>>> GetRolesPagedAsync(PaginationParams paginationParams)
        {
            var query = _context.Roles
                .Where(r => !r.IsDeleted)
                .AsQueryable();

            // Aplicar filtrado y ordenación
            if (!string.IsNullOrEmpty(paginationParams.SortBy))
            {
                var sortDirection = paginationParams.SortDirection?.ToLower() == "desc" ? "descending" : "ascending";
                var sortProperty = paginationParams.SortBy;
                
                // Asegurarse de que la propiedad existe
                if (typeof(Role).GetProperty(sortProperty) != null)
                {
                    query = query.OrderBy($"{sortProperty} {sortDirection}");
                }
                else
                {
                    query = query.OrderBy(r => r.Id);
                }
            }
            else
            {
                query = query.OrderBy(r => r.Id);
            }

            var paginatedRoles = await PaginatedList<RoleDto>.CreateAsync(
                query.Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    CreatedAt = r.CreatedAt,
                    CreatedBy = r.CreatedBy,
                    UpdatedAt = r.UpdatedAt,
                    UpdatedBy = r.UpdatedBy
                }),
                paginationParams.PageNumber,
                paginationParams.PageSize);

            return Result<PaginatedList<RoleDto>>.Success(paginatedRoles);
        }

        public async Task<Result<List<RoleDto>>> GetAllRolesAsync()
        {
            var roles = await _context.Roles
                .Where(r => !r.IsDeleted)
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    CreatedAt = r.CreatedAt,
                    CreatedBy = r.CreatedBy,
                    UpdatedAt = r.UpdatedAt,
                    UpdatedBy = r.UpdatedBy
                })
                .ToListAsync();

            return Result<List<RoleDto>>.Success(roles);
        }

        public async Task<Result<RoleDto>> GetRoleByIdAsync(Guid id)
        {
            var role = await _context.Roles
                .Where(r => r.Id == id && !r.IsDeleted)
                .Select(r => new RoleDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    CreatedAt = r.CreatedAt,
                    CreatedBy = r.CreatedBy,
                    UpdatedAt = r.UpdatedAt,
                    UpdatedBy = r.UpdatedBy
                })
                .FirstOrDefaultAsync();

            if (role == null)
            {
                return Result<RoleDto>.Failure("El rol especificado no existe.");
            }

            return Result<RoleDto>.Success(role);
        }

        public async Task<Result<RoleDto>> CreateRoleAsync(CreateRoleDto createRoleDto)
        {
            // Verificar si ya existe un rol con el mismo nombre
            var existingRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name.ToLower() == createRoleDto.Name.ToLower() && !r.IsDeleted);

            if (existingRole != null)
            {
                return Result<RoleDto>.Failure($"Ya existe un rol con el nombre '{createRoleDto.Name}'.");
            }

            var role = new Role
            {
                Id = Guid.NewGuid(),
                Name = createRoleDto.Name,
                Description = createRoleDto.Description,
                IsDeleted = false,
                CreatedBy = _currentUserService.UserId ?? "System",
                CreatedAt = _dateTime.Now,
                UpdatedBy = _currentUserService.UserId ?? "System",
                UpdatedAt = _dateTime.Now
            };

            await _context.Roles.AddAsync(role);
            await _context.SaveChangesAsync(default);

            var roleDto = new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                CreatedAt = role.CreatedAt,
                CreatedBy = role.CreatedBy,
                UpdatedAt = role.UpdatedAt,
                UpdatedBy = role.UpdatedBy
            };

            return Result<RoleDto>.Success(roleDto);
        }

        public async Task<Result<RoleDto>> UpdateRoleAsync(UpdateRoleDto updateRoleDto)
        {
            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == updateRoleDto.Id && !r.IsDeleted);

            if (role == null)
            {
                return Result<RoleDto>.Failure("El rol especificado no existe.");
            }

            // Verificar si ya existe otro rol con el mismo nombre
            var existingRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name.ToLower() == updateRoleDto.Name.ToLower() 
                                      && r.Id != updateRoleDto.Id 
                                      && !r.IsDeleted);

            if (existingRole != null)
            {
                return Result<RoleDto>.Failure($"Ya existe otro rol con el nombre '{updateRoleDto.Name}'.");
            }

            role.Name = updateRoleDto.Name;
            role.Description = updateRoleDto.Description;
            role.UpdatedBy = _currentUserService.UserId ?? "System";
            role.UpdatedAt = _dateTime.Now;

            _context.Roles.Update(role);
            await _context.SaveChangesAsync(default);

            var roleDto = new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Description = role.Description,
                CreatedAt = role.CreatedAt,
                CreatedBy = role.CreatedBy,
                UpdatedAt = role.UpdatedAt,
                UpdatedBy = role.UpdatedBy
            };

            return Result<RoleDto>.Success(roleDto);
        }

        public async Task<Result<bool>> DeleteRoleAsync(Guid id)
        {
            var role = await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted);

            if (role == null)
            {
                return Result<bool>.Failure("El rol especificado no existe.");
            }

            // Verificar si hay usuarios asignados a este rol
            var hasUsers = await _context.UserRoles
                .AnyAsync(ur => ur.RoleId == id && !ur.IsDeleted);

            if (hasUsers)
            {
                return Result<bool>.Failure("No se puede eliminar el rol porque tiene usuarios asignados.");
            }

            // Eliminar lógicamente el rol
            role.IsDeleted = true;
            role.UpdatedBy = _currentUserService.UserId ?? "System";
            role.UpdatedAt = _dateTime.Now;

            _context.Roles.Update(role);
            await _context.SaveChangesAsync(default);

            return Result<bool>.Success(true);
        }

        public async Task<Result<bool>> AssignModuleToRoleAsync(AssignModuleToRoleDto assignModuleToRoleDto)
        {
            try
            {
                // Verificar si el rol existe
                var role = await _context.Roles
                    .FirstOrDefaultAsync(r => r.Id == assignModuleToRoleDto.RoleId && !r.IsDeleted);

                if (role == null)
                {
                    return Result<bool>.Failure("El rol especificado no existe.");
                }

                // Verificar si el módulo existe
                var module = await _context.Modules
                    .FirstOrDefaultAsync(m => m.Id == assignModuleToRoleDto.ModuleId && !m.IsDeleted);

                if (module == null)
                {
                    return Result<bool>.Failure("El módulo especificado no existe.");
                }

                // Verificar si ya existe la asignación
                var existingAssignment = await _context.RoleModules
                    .FirstOrDefaultAsync(rm => rm.RoleId == assignModuleToRoleDto.RoleId && 
                                              rm.ModuleId == assignModuleToRoleDto.ModuleId && 
                                              !rm.IsDeleted);

                if (existingAssignment != null)
                {
                    // Si ya existe pero está inactivo, lo activamos
                    if (!existingAssignment.IsActive)
                    {
                        existingAssignment.IsActive = true;
                        existingAssignment.UpdatedBy = _currentUserService.UserId ?? "System";
                        existingAssignment.UpdatedAt = _dateTime.Now;

                        _context.RoleModules.Update(existingAssignment);
                        await _context.SaveChangesAsync(default);

                        return Result<bool>.Success(true);
                    }

                    return Result<bool>.Failure("El módulo ya está asignado a este rol.");
                }

                // Crear la asignación
                var roleModule = new RoleModule
                {
                    Id = Guid.NewGuid(),
                    RoleId = assignModuleToRoleDto.RoleId,
                    ModuleId = assignModuleToRoleDto.ModuleId,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = _currentUserService.UserId ?? "System",
                    CreatedAt = _dateTime.Now,
                    UpdatedBy = _currentUserService.UserId ?? "System",
                    UpdatedAt = _dateTime.Now
                };

                await _context.RoleModules.AddAsync(roleModule);
                await _context.SaveChangesAsync(default);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Error al asignar el módulo al rol: {ex.Message}");
            }
        }

        public async Task<Result<bool>> RemoveModuleFromRoleAsync(Guid roleId, Guid moduleId)
        {
            try
            {
                // Verificar si existe la asignación
                var roleModule = await _context.RoleModules
                    .FirstOrDefaultAsync(rm => rm.RoleId == roleId && 
                                              rm.ModuleId == moduleId && 
                                              !rm.IsDeleted);

                if (roleModule == null)
                {
                    return Result<bool>.Failure("El módulo no está asignado a este rol.");
                }

                // Marcar como eliminado
                roleModule.IsDeleted = true;
                roleModule.UpdatedBy = _currentUserService.UserId ?? "System";
                roleModule.UpdatedAt = _dateTime.Now;

                _context.RoleModules.Update(roleModule);
                await _context.SaveChangesAsync(default);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Error al eliminar el módulo del rol: {ex.Message}");
            }
        }

        public async Task<Result<bool>> AssignRouteToRoleAsync(AssignRouteToRoleDto assignRouteToRoleDto)
        {
            try
            {
                // Verificar si el rol existe
                var role = await _context.Roles
                    .FirstOrDefaultAsync(r => r.Id == assignRouteToRoleDto.RoleId && !r.IsDeleted);

                if (role == null)
                {
                    return Result<bool>.Failure("El rol especificado no existe.");
                }

                // Verificar si la ruta existe
                var route = await _context.Routes
                    .FirstOrDefaultAsync(r => r.Id == assignRouteToRoleDto.RouteId && !r.IsDeleted);

                if (route == null)
                {
                    return Result<bool>.Failure("La ruta especificada no existe.");
                }

                // Verificar si ya existe la asignación
                var existingAssignment = await _context.RoleRoutes
                    .FirstOrDefaultAsync(rr => rr.RoleId == assignRouteToRoleDto.RoleId && 
                                              rr.RouteId == assignRouteToRoleDto.RouteId && 
                                              !rr.IsDeleted);

                if (existingAssignment != null)
                {
                    // Si ya existe pero está inactivo, lo activamos
                    if (!existingAssignment.IsActive)
                    {
                        existingAssignment.IsActive = true;
                        existingAssignment.UpdatedBy = _currentUserService.UserId ?? "System";
                        existingAssignment.UpdatedAt = _dateTime.Now;

                        _context.RoleRoutes.Update(existingAssignment);
                        await _context.SaveChangesAsync(default);

                        return Result<bool>.Success(true);
                    }

                    return Result<bool>.Failure("La ruta ya está asignada a este rol.");
                }

                // Crear la asignación
                var roleRoute = new RoleRoute
                {
                    Id = Guid.NewGuid(),
                    RoleId = assignRouteToRoleDto.RoleId,
                    RouteId = assignRouteToRoleDto.RouteId,
                    IsActive = true,
                    IsDeleted = false,
                    CreatedBy = _currentUserService.UserId ?? "System",
                    CreatedAt = _dateTime.Now,
                    UpdatedBy = _currentUserService.UserId ?? "System",
                    UpdatedAt = _dateTime.Now
                };

                await _context.RoleRoutes.AddAsync(roleRoute);
                await _context.SaveChangesAsync(default);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Error al asignar la ruta al rol: {ex.Message}");
            }
        }

        public async Task<Result<bool>> RemoveRouteFromRoleAsync(Guid roleId, Guid routeId)
        {
            try
            {
                // Verificar si existe la asignación
                var roleRoute = await _context.RoleRoutes
                    .FirstOrDefaultAsync(rr => rr.RoleId == roleId && 
                                              rr.RouteId == routeId && 
                                              !rr.IsDeleted);

                if (roleRoute == null)
                {
                    return Result<bool>.Failure("La ruta no está asignada a este rol.");
                }

                // Marcar como eliminado
                roleRoute.IsDeleted = true;
                roleRoute.UpdatedBy = _currentUserService.UserId ?? "System";
                roleRoute.UpdatedAt = _dateTime.Now;

                _context.RoleRoutes.Update(roleRoute);
                await _context.SaveChangesAsync(default);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Error al eliminar la ruta del rol: {ex.Message}");
            }
        }
    }
}
