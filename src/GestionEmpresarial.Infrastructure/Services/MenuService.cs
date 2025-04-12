using GestionEmpresarial.Application.Common.Interfaces;
using GestionEmpresarial.Application.Common.Models;
using GestionEmpresarial.Application.Menus.Dtos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestionEmpresarial.Infrastructure.Services
{
    public class MenuService : IMenuService
    {
        private readonly IApplicationDbContext _context;

        public MenuService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<MenuDto>>> GetMenuByUserIdAsync(Guid userId)
        {
            try
            {
                // Obtener los roles del usuario
                var userRoles = await _context.UserRoles
                    .Where(ur => ur.UserId == userId && ur.IsActive && !ur.IsDeleted)
                    .Select(ur => ur.RoleId)
                    .ToListAsync();

                if (userRoles.Count == 0)
                {
                    return Result<List<MenuDto>>.Success(new List<MenuDto>());
                }

                // Obtener los módulos asignados a los roles del usuario
                var roleModules = await _context.RoleModules
                    .Where(rm => userRoles.Contains(rm.RoleId) && rm.IsActive && !rm.IsDeleted)
                    .Select(rm => rm.ModuleId)
                    .Distinct()
                    .ToListAsync();

                // Obtener los módulos con sus detalles
                var modules = await _context.Modules
                    .Where(m => roleModules.Contains(m.Id) && m.IsActive && !m.IsDeleted)
                    .OrderBy(m => m.Order)
                    .ToListAsync();

                // Obtener las rutas asignadas a los roles del usuario
                var roleRoutes = await _context.RoleRoutes
                    .Where(rr => userRoles.Contains(rr.RoleId) && rr.IsActive && !rr.IsDeleted)
                    .Select(rr => rr.RouteId)
                    .Distinct()
                    .ToListAsync();

                // Obtener las rutas con sus detalles
                var routes = await _context.Routes
                    .Where(r => roleRoutes.Contains(r.Id) && r.IsActive && !r.IsDeleted)
                    .Include(r => r.Module)
                    .OrderBy(r => r.Order)
                    .ToListAsync();

                // Construir el menú
                var menu = new List<MenuDto>();

                foreach (var module in modules)
                {
                    var menuItem = new MenuDto
                    {
                        Id = module.Id,
                        Name = module.Name,
                        Icon = module.Icon,
                        Path = module.Path,
                        Order = module.Order,
                        IsActive = module.IsActive,
                        Children = routes
                            .Where(r => r.ModuleId == module.Id)
                            .Select(r => new MenuItemDto
                            {
                                Id = r.Id,
                                Name = r.Name,
                                Icon = r.Icon,
                                Path = r.Path,
                                Order = r.Order,
                                IsActive = r.IsActive
                            })
                            .OrderBy(r => r.Order)
                            .ToList()
                    };

                    menu.Add(menuItem);
                }

                return Result<List<MenuDto>>.Success(menu);
            }
            catch (Exception ex)
            {
                return Result<List<MenuDto>>.Failure($"Error al obtener el menú del usuario: {ex.Message}");
            }
        }

        public async Task<Result<List<MenuDto>>> GetMenuByRoleIdAsync(Guid roleId)
        {
            try
            {
                // Verificar si el rol existe
                var roleExists = await _context.Roles
                    .AnyAsync(r => r.Id == roleId && !r.IsDeleted);

                if (!roleExists)
                {
                    return Result<List<MenuDto>>.Failure("El rol especificado no existe.");
                }

                // Obtener los módulos asignados al rol
                var roleModules = await _context.RoleModules
                    .Where(rm => rm.RoleId == roleId && rm.IsActive && !rm.IsDeleted)
                    .Select(rm => rm.ModuleId)
                    .ToListAsync();

                // Obtener los módulos con sus detalles
                var modules = await _context.Modules
                    .Where(m => roleModules.Contains(m.Id) && m.IsActive && !m.IsDeleted)
                    .OrderBy(m => m.Order)
                    .ToListAsync();

                // Obtener las rutas asignadas al rol
                var roleRoutes = await _context.RoleRoutes
                    .Where(rr => rr.RoleId == roleId && rr.IsActive && !rr.IsDeleted)
                    .Select(rr => rr.RouteId)
                    .ToListAsync();

                // Obtener las rutas con sus detalles
                var routes = await _context.Routes
                    .Where(r => roleRoutes.Contains(r.Id) && r.IsActive && !r.IsDeleted)
                    .Include(r => r.Module)
                    .OrderBy(r => r.Order)
                    .ToListAsync();

                // Construir el menú
                var menu = new List<MenuDto>();

                foreach (var module in modules)
                {
                    var menuItem = new MenuDto
                    {
                        Id = module.Id,
                        Name = module.Name,
                        Icon = module.Icon,
                        Path = module.Path,
                        Order = module.Order,
                        IsActive = module.IsActive,
                        Children = routes
                            .Where(r => r.ModuleId == module.Id)
                            .Select(r => new MenuItemDto
                            {
                                Id = r.Id,
                                Name = r.Name,
                                Icon = r.Icon,
                                Path = r.Path,
                                Order = r.Order,
                                IsActive = r.IsActive
                            })
                            .OrderBy(r => r.Order)
                            .ToList()
                    };

                    menu.Add(menuItem);
                }

                return Result<List<MenuDto>>.Success(menu);
            }
            catch (Exception ex)
            {
                return Result<List<MenuDto>>.Failure($"Error al obtener el menú del rol: {ex.Message}");
            }
        }
    }
}
