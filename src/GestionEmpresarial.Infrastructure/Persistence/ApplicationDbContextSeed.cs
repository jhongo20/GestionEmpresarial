using GestionEmpresarial.Domain.Entities;
using GestionEmpresarial.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestionEmpresarial.Infrastructure.Persistence
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedDefaultDataAsync(ApplicationDbContext context, ILogger<ApplicationDbContext> logger)
        {
            try
            {
                // Sembrar roles si no existen
                if (!context.Roles.Any())
                {
                    await SeedRolesAsync(context);
                    await context.SaveChangesAsync();
                    logger.LogInformation("Roles sembrados correctamente.");
                }

                // Sembrar permisos si no existen
                if (!context.Permissions.Any())
                {
                    await SeedPermissionsAsync(context);
                    await context.SaveChangesAsync();
                    logger.LogInformation("Permisos sembrados correctamente.");
                }

                // Asignar permisos a roles si no existen asignaciones
                if (!context.RolePermissions.Any())
                {
                    await SeedRolePermissionsAsync(context);
                    await context.SaveChangesAsync();
                    logger.LogInformation("Asignaciones de permisos a roles sembradas correctamente.");
                }

                // Sembrar usuarios si no existen
                if (!context.Users.Any())
                {
                    await SeedUsersAsync(context);
                    await context.SaveChangesAsync();
                    logger.LogInformation("Usuarios sembrados correctamente.");
                }

                // Asignar roles a usuarios si no existen asignaciones
                if (!context.UserRoles.Any())
                {
                    await SeedUserRolesAsync(context);
                    await context.SaveChangesAsync();
                    logger.LogInformation("Asignaciones de roles a usuarios sembradas correctamente.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error sembrando datos por defecto.");
                throw;
            }
        }

        private static async Task SeedRolesAsync(ApplicationDbContext context)
        {
            var roles = new List<Role>
            {
                new Role
                {
                    Id = Guid.NewGuid(),
                    Name = "Admin",
                    Description = "Administrador del sistema con acceso total",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new Role
                {
                    Id = Guid.NewGuid(),
                    Name = "Manager",
                    Description = "Gerente con acceso a gestión de la empresa",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new Role
                {
                    Id = Guid.NewGuid(),
                    Name = "Employee",
                    Description = "Empleado con acceso básico",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                }
            };

            await context.Roles.AddRangeAsync(roles);
        }

        private static async Task SeedPermissionsAsync(ApplicationDbContext context)
        {
            var permissions = new List<Permission>
            {
                // Permisos para Empresas
                new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = "companies.view",
                    Description = "Ver empresas",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = "companies.create",
                    Description = "Crear empresas",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = "companies.edit",
                    Description = "Editar empresas",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = "companies.delete",
                    Description = "Eliminar empresas",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                
                // Permisos para Sucursales
                new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = "branches.view",
                    Description = "Ver sucursales",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = "branches.create",
                    Description = "Crear sucursales",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = "branches.edit",
                    Description = "Editar sucursales",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = "branches.delete",
                    Description = "Eliminar sucursales",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                
                // Permisos para Usuarios
                new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = "users.view",
                    Description = "Ver usuarios",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = "users.create",
                    Description = "Crear usuarios",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = "users.edit",
                    Description = "Editar usuarios",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new Permission
                {
                    Id = Guid.NewGuid(),
                    Name = "users.delete",
                    Description = "Eliminar usuarios",
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                }
            };

            await context.Permissions.AddRangeAsync(permissions);
        }

        private static async Task SeedRolePermissionsAsync(ApplicationDbContext context)
        {
            // Obtener roles
            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
            var managerRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Manager");
            var employeeRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Employee");

            // Obtener todos los permisos
            var permissions = await context.Permissions.ToListAsync();

            // Asignar todos los permisos al rol Admin
            var adminPermissions = permissions.Select(p => new RolePermission
            {
                Id = Guid.NewGuid(),
                RoleId = adminRole.Id,
                PermissionId = p.Id,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            }).ToList();

            // Asignar permisos de visualización y edición al rol Manager
            var managerPermissions = permissions
                .Where(p => p.Name.EndsWith(".view") || p.Name.EndsWith(".edit") || p.Name.EndsWith(".create"))
                .Select(p => new RolePermission
                {
                    Id = Guid.NewGuid(),
                    RoleId = managerRole.Id,
                    PermissionId = p.Id,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                }).ToList();

            // Asignar solo permisos de visualización al rol Employee
            var employeePermissions = permissions
                .Where(p => p.Name.EndsWith(".view"))
                .Select(p => new RolePermission
                {
                    Id = Guid.NewGuid(),
                    RoleId = employeeRole.Id,
                    PermissionId = p.Id,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                }).ToList();

            await context.RolePermissions.AddRangeAsync(adminPermissions);
            await context.RolePermissions.AddRangeAsync(managerPermissions);
            await context.RolePermissions.AddRangeAsync(employeePermissions);
        }

        private static async Task SeedUsersAsync(ApplicationDbContext context)
        {
            var users = new List<User>
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "admin",
                    Email = "admin@gestionempresarial.com",
                    PasswordHash = "Admin123!", // En producción, esto debería estar hasheado
                    FirstName = "Admin",
                    LastName = "User",
                    Status = UserStatus.Active,
                    UserType = UserType.Internal,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "manager",
                    Email = "manager@gestionempresarial.com",
                    PasswordHash = "Manager123!", // En producción, esto debería estar hasheado
                    FirstName = "Manager",
                    LastName = "User",
                    Status = UserStatus.Active,
                    UserType = UserType.Internal,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Username = "employee",
                    Email = "employee@gestionempresarial.com",
                    PasswordHash = "Employee123!", // En producción, esto debería estar hasheado
                    FirstName = "Employee",
                    LastName = "User",
                    Status = UserStatus.Active,
                    UserType = UserType.Internal,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                }
            };

            await context.Users.AddRangeAsync(users);
        }

        private static async Task SeedUserRolesAsync(ApplicationDbContext context)
        {
            // Obtener usuarios
            var adminUser = await context.Users.FirstOrDefaultAsync(u => u.Username == "admin");
            var managerUser = await context.Users.FirstOrDefaultAsync(u => u.Username == "manager");
            var employeeUser = await context.Users.FirstOrDefaultAsync(u => u.Username == "employee");

            // Obtener roles
            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
            var managerRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Manager");
            var employeeRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == "Employee");

            // Crear asignaciones de roles a usuarios
            var userRoles = new List<UserRole>
            {
                new UserRole
                {
                    Id = Guid.NewGuid(),
                    UserId = adminUser.Id,
                    RoleId = adminRole.Id,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new UserRole
                {
                    Id = Guid.NewGuid(),
                    UserId = managerUser.Id,
                    RoleId = managerRole.Id,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new UserRole
                {
                    Id = Guid.NewGuid(),
                    UserId = employeeUser.Id,
                    RoleId = employeeRole.Id,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                }
            };

            await context.UserRoles.AddRangeAsync(userRoles);
        }
    }
}
