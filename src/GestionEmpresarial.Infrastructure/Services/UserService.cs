using GestionEmpresarial.Application.Common.Interfaces;
using GestionEmpresarial.Application.Common.Models;
using GestionEmpresarial.Application.Users.Dtos;
using GestionEmpresarial.Domain.Entities;
using GestionEmpresarial.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using BCrypt.Net;

namespace GestionEmpresarial.Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTime _dateTime;
        private readonly ILogger<UserService> _logger;
        private readonly IEmailService _emailService;

        public UserService(
            IApplicationDbContext context,
            ICurrentUserService currentUserService,
            IDateTime dateTime,
            ILogger<UserService> logger,
            IEmailService emailService)
        {
            _context = context;
            _currentUserService = currentUserService;
            _dateTime = dateTime;
            _logger = logger;
            _emailService = emailService;
        }

        public async Task<Result<List<UserDto>>> GetAllUsersAsync()
        {
            var users = await _context.Users
                .Where(u => !u.IsDeleted)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    PhoneNumber = u.PhoneNumber,
                    LastLoginDate = u.LastLoginDate,
                    Status = u.Status,
                    UserType = u.UserType,
                    Roles = u.UserRoles
                        .Where(ur => !ur.IsDeleted)
                        .Select(ur => ur.Role.Name)
                        .ToList(),
                    CreatedAt = u.CreatedAt,
                    CreatedBy = u.CreatedBy,
                    UpdatedAt = u.UpdatedAt,
                    UpdatedBy = u.UpdatedBy
                })
                .ToListAsync();

            return Result<List<UserDto>>.Success(users);
        }

        public async Task<Result<PaginatedList<UserDto>>> GetUsersPagedAsync(PaginationParams paginationParams)
        {
            var query = _context.Users
                .Where(u => !u.IsDeleted)
                .AsQueryable();

            // Aplicar filtrado y ordenación
            if (!string.IsNullOrEmpty(paginationParams.SortBy))
            {
                var sortDirection = paginationParams.SortDirection?.ToLower() == "desc" ? "descending" : "ascending";
                var sortProperty = paginationParams.SortBy;
                
                // Asegurarse de que la propiedad existe
                if (typeof(User).GetProperty(sortProperty) != null)
                {
                    query = query.OrderBy($"{sortProperty} {sortDirection}");
                }
                else
                {
                    query = query.OrderBy(u => u.Id);
                }
            }
            else
            {
                query = query.OrderBy(u => u.Id);
            }

            var paginatedUsers = await PaginatedList<UserDto>.CreateAsync(
                query.Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    PhoneNumber = u.PhoneNumber,
                    LastLoginDate = u.LastLoginDate,
                    Status = u.Status,
                    UserType = u.UserType,
                    Roles = u.UserRoles
                        .Where(ur => !ur.IsDeleted)
                        .Select(ur => ur.Role.Name)
                        .ToList(),
                    CreatedAt = u.CreatedAt,
                    CreatedBy = u.CreatedBy,
                    UpdatedAt = u.UpdatedAt,
                    UpdatedBy = u.UpdatedBy
                }),
                paginationParams.PageNumber,
                paginationParams.PageSize);

            return Result<PaginatedList<UserDto>>.Success(paginatedUsers);
        }

        public async Task<Result<PaginatedList<UserDto>>> GetUsersByRolePagedAsync(Guid roleId, PaginationParams paginationParams)
        {
            // Verificar si el rol existe
            var roleExists = await _context.Roles.AnyAsync(r => r.Id == roleId && !r.IsDeleted);
            if (!roleExists)
            {
                return Result<PaginatedList<UserDto>>.Failure($"El rol con ID {roleId} no existe.");
            }

            var query = _context.Users
                .Where(u => !u.IsDeleted)
                .Where(u => u.UserRoles.Any(ur => ur.RoleId == roleId && !ur.IsDeleted))
                .AsQueryable();

            // Aplicar filtrado y ordenación
            if (!string.IsNullOrEmpty(paginationParams.SortBy))
            {
                var sortDirection = paginationParams.SortDirection?.ToLower() == "desc" ? "descending" : "ascending";
                var sortProperty = paginationParams.SortBy;
                
                // Asegurarse de que la propiedad existe
                if (typeof(User).GetProperty(sortProperty) != null)
                {
                    query = query.OrderBy($"{sortProperty} {sortDirection}");
                }
                else
                {
                    query = query.OrderBy(u => u.Id);
                }
            }
            else
            {
                query = query.OrderBy(u => u.Id);
            }

            var paginatedUsers = await PaginatedList<UserDto>.CreateAsync(
                query.Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    PhoneNumber = u.PhoneNumber,
                    LastLoginDate = u.LastLoginDate,
                    Status = u.Status,
                    UserType = u.UserType,
                    Roles = u.UserRoles
                        .Where(ur => !ur.IsDeleted)
                        .Select(ur => ur.Role.Name)
                        .ToList(),
                    CreatedAt = u.CreatedAt,
                    CreatedBy = u.CreatedBy,
                    UpdatedAt = u.UpdatedAt,
                    UpdatedBy = u.UpdatedBy
                }),
                paginationParams.PageNumber,
                paginationParams.PageSize);

            return Result<PaginatedList<UserDto>>.Success(paginatedUsers);
        }

        public async Task<Result<UserDto>> GetUserByIdAsync(Guid id)
        {
            var user = await _context.Users
                .Where(u => u.Id == id && !u.IsDeleted)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Email = u.Email,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    PhoneNumber = u.PhoneNumber,
                    LastLoginDate = u.LastLoginDate,
                    Status = u.Status,
                    UserType = u.UserType,
                    Roles = u.UserRoles
                        .Where(ur => !ur.IsDeleted)
                        .Select(ur => ur.Role.Name)
                        .ToList(),
                    CreatedAt = u.CreatedAt,
                    CreatedBy = u.CreatedBy,
                    UpdatedAt = u.UpdatedAt,
                    UpdatedBy = u.UpdatedBy
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return Result<UserDto>.Failure("El usuario especificado no existe.");
            }

            return Result<UserDto>.Success(user);
        }

        public async Task<Result<UserDto>> CreateUserAsync(CreateUserDto createUserDto)
        {
            try
            {
                // Verificar si el nombre de usuario ya existe
                if (await _context.Users.AnyAsync(u => u.Username == createUserDto.Username && !u.IsDeleted))
                {
                    return Result<UserDto>.Failure("El nombre de usuario ya está en uso.");
                }

                // Verificar si el correo electrónico ya existe
                if (await _context.Users.AnyAsync(u => u.Email == createUserDto.Email && !u.IsDeleted))
                {
                    return Result<UserDto>.Failure("El correo electrónico ya está en uso.");
                }

                // Verificar si los roles existen
                if (createUserDto.RoleIds.Any())
                {
                    var existingRoleIds = await _context.Roles
                        .Where(r => createUserDto.RoleIds.Contains(r.Id) && !r.IsDeleted)
                        .Select(r => r.Id)
                        .ToListAsync();

                    var nonExistingRoleIds = createUserDto.RoleIds.Except(existingRoleIds).ToList();
                    if (nonExistingRoleIds.Any())
                    {
                        return Result<UserDto>.Failure($"Los siguientes roles no existen: {string.Join(", ", nonExistingRoleIds)}");
                    }
                }

                // Crear el usuario
                var user = new User
                {
                    Id = Guid.NewGuid(),
                    Username = createUserDto.Username,
                    Email = createUserDto.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(createUserDto.Password), // Usar BCrypt para hashear la contraseña
                    FirstName = createUserDto.FirstName,
                    LastName = createUserDto.LastName,
                    PhoneNumber = createUserDto.PhoneNumber,
                    Status = createUserDto.Status,
                    UserType = createUserDto.UserType,
                    IsDeleted = false,
                    CreatedBy = _currentUserService.UserId ?? "System",
                    CreatedAt = _dateTime.Now,
                    UpdatedBy = _currentUserService.UserId ?? "System",
                    UpdatedAt = _dateTime.Now
                };

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync(default);

                // Asignar roles al usuario
                if (createUserDto.RoleIds.Any())
                {
                    var userRoles = createUserDto.RoleIds.Select(roleId => new UserRole
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        RoleId = roleId,
                        IsDeleted = false,
                        CreatedBy = _currentUserService.UserId ?? "System",
                        CreatedAt = _dateTime.Now,
                        UpdatedBy = _currentUserService.UserId ?? "System",
                        UpdatedAt = _dateTime.Now
                    }).ToList();

                    await _context.UserRoles.AddRangeAsync(userRoles);
                    await _context.SaveChangesAsync(default);
                }

                // Enviar correo de confirmación de registro
                try
                {
                    await _emailService.SendRegistrationConfirmationEmailAsync(user.Email, user.Username);
                    _logger.LogInformation($"Correo de confirmación enviado a {user.Email}");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error al enviar correo de confirmación a {user.Email}: {ex.Message}");
                    // No interrumpimos el flujo si falla el envío de correo
                }

                // Obtener los nombres de los roles asignados
                var roleNames = await _context.Roles
                    .Where(r => createUserDto.RoleIds.Contains(r.Id) && !r.IsDeleted)
                    .Select(r => r.Name)
                    .ToListAsync();

                var userDto = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    LastLoginDate = user.LastLoginDate,
                    Status = user.Status,
                    UserType = user.UserType,
                    Roles = roleNames,
                    CreatedAt = user.CreatedAt,
                    CreatedBy = user.CreatedBy,
                    UpdatedAt = user.UpdatedAt,
                    UpdatedBy = user.UpdatedBy
                };

                return Result<UserDto>.Success(userDto);
            }
            catch (Exception ex)
            {
                return Result<UserDto>.Failure("Error al crear el usuario: " + ex.Message);
            }
        }

        public async Task<Result<UserDto>> UpdateUserAsync(UpdateUserDto updateUserDto)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .FirstOrDefaultAsync(u => u.Id == updateUserDto.Id && !u.IsDeleted);

            if (user == null)
            {
                return Result<UserDto>.Failure("El usuario especificado no existe.");
            }

            // Verificar si ya existe otro usuario con el mismo correo electrónico
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == updateUserDto.Email.ToLower() 
                                      && u.Id != updateUserDto.Id 
                                      && !u.IsDeleted);

            if (existingUser != null)
            {
                return Result<UserDto>.Failure($"Ya existe otro usuario con el correo electrónico '{updateUserDto.Email}'.");
            }

            // Verificar que los roles existan
            if (updateUserDto.RoleIds.Any())
            {
                var existingRoleIds = await _context.Roles
                    .Where(r => updateUserDto.RoleIds.Contains(r.Id) && !r.IsDeleted)
                    .Select(r => r.Id)
                    .ToListAsync();

                var nonExistingRoleIds = updateUserDto.RoleIds.Except(existingRoleIds).ToList();
                if (nonExistingRoleIds.Any())
                {
                    return Result<UserDto>.Failure($"Los siguientes roles no existen: {string.Join(", ", nonExistingRoleIds)}");
                }
            }

            // Actualizar el usuario
            user.Email = updateUserDto.Email;
            user.FirstName = updateUserDto.FirstName;
            user.LastName = updateUserDto.LastName;
            user.PhoneNumber = updateUserDto.PhoneNumber;
            user.Status = updateUserDto.Status;
            user.UserType = updateUserDto.UserType;
            user.UpdatedBy = _currentUserService.UserId ?? "System";
            user.UpdatedAt = _dateTime.Now;

            _context.Users.Update(user);
            await _context.SaveChangesAsync(default);

            // Actualizar roles si es necesario
            if (updateUserDto.RoleIds != null)
            {
                // Eliminar roles existentes
                var existingUserRoles = await _context.UserRoles
                    .Where(ur => ur.UserId == user.Id && !ur.IsDeleted)
                    .ToListAsync();

                foreach (var userRole in existingUserRoles)
                {
                    userRole.IsDeleted = true;
                    userRole.UpdatedBy = _currentUserService.UserId ?? "System";
                    userRole.UpdatedAt = _dateTime.Now;
                }

                // Asignar nuevos roles
                if (updateUserDto.RoleIds.Any())
                {
                    var newUserRoles = updateUserDto.RoleIds.Select(roleId => new UserRole
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        RoleId = roleId,
                        IsDeleted = false,
                        CreatedBy = _currentUserService.UserId ?? "System",
                        CreatedAt = _dateTime.Now,
                        UpdatedBy = _currentUserService.UserId ?? "System",
                        UpdatedAt = _dateTime.Now
                    }).ToList();

                    await _context.UserRoles.AddRangeAsync(newUserRoles);
                }

                await _context.SaveChangesAsync(default);
            }

            // Obtener los nombres de los roles asignados
            var roleNames = await _context.Roles
                .Join(_context.UserRoles.Where(ur => ur.UserId == user.Id && !ur.IsDeleted),
                    r => r.Id,
                    ur => ur.RoleId,
                    (r, ur) => r.Name)
                .ToListAsync();

            var userDto = new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                LastLoginDate = user.LastLoginDate,
                Status = user.Status,
                UserType = user.UserType,
                Roles = roleNames,
                CreatedAt = user.CreatedAt,
                CreatedBy = user.CreatedBy,
                UpdatedAt = user.UpdatedAt,
                UpdatedBy = user.UpdatedBy
            };

            // Enviar correo de notificación de actualización
            try
            {
                await _emailService.SendAccountUpdatedEmailAsync(user.Email, user.Username);
            }
            catch (Exception ex)
            {
                // Loguear el error pero no fallar la operación
                _logger.LogError(ex, $"Error al enviar correo de notificación de actualización a {user.Email}");
            }

            return Result<UserDto>.Success(userDto);
        }

        public async Task<Result> DeleteUserAsync(Guid id)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id && !u.IsDeleted);

            if (user == null)
            {
                return Result.Failure("El usuario especificado no existe.");
            }

            // Eliminar lógicamente el usuario
            user.IsDeleted = true;
            user.UpdatedBy = _currentUserService.UserId ?? "System";
            user.UpdatedAt = _dateTime.Now;

            _context.Users.Update(user);
            await _context.SaveChangesAsync(default);

            return Result.Success();
        }

        public async Task<Result> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == changePasswordDto.UserId && !u.IsDeleted);

            if (user == null)
            {
                return Result.Failure("El usuario especificado no existe.");
            }

            // Verificar que la contraseña actual sea correcta
            if (!BCrypt.Net.BCrypt.Verify(changePasswordDto.CurrentPassword, user.PasswordHash))
            {
                return Result.Failure("La contraseña actual es incorrecta.");
            }

            // Verificar que la nueva contraseña y la confirmación coincidan
            if (changePasswordDto.NewPassword != changePasswordDto.ConfirmPassword)
            {
                return Result.Failure("La nueva contraseña y la confirmación no coinciden.");
            }

            // Actualizar la contraseña
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(changePasswordDto.NewPassword);
            user.UpdatedBy = _currentUserService.UserId ?? "System";
            user.UpdatedAt = _dateTime.Now;

            _context.Users.Update(user);
            await _context.SaveChangesAsync(default);

            return Result.Success();
        }

        public async Task<Result<List<UserDto>>> GetUsersByRoleAsync(Guid roleId)
        {
            // Verificar que el rol exista
            var roleExists = await _context.Roles
                .AnyAsync(r => r.Id == roleId && !r.IsDeleted);

            if (!roleExists)
            {
                return Result<List<UserDto>>.Failure("El rol especificado no existe.");
            }

            var users = await _context.UserRoles
                .Where(ur => ur.RoleId == roleId && !ur.IsDeleted && !ur.User.IsDeleted)
                .Select(ur => new UserDto
                {
                    Id = ur.User.Id,
                    Username = ur.User.Username,
                    Email = ur.User.Email,
                    FirstName = ur.User.FirstName,
                    LastName = ur.User.LastName,
                    PhoneNumber = ur.User.PhoneNumber,
                    LastLoginDate = ur.User.LastLoginDate,
                    Status = ur.User.Status,
                    UserType = ur.User.UserType,
                    Roles = ur.User.UserRoles
                        .Where(ur2 => !ur2.IsDeleted)
                        .Select(ur2 => ur2.Role.Name)
                        .ToList(),
                    CreatedAt = ur.User.CreatedAt,
                    CreatedBy = ur.User.CreatedBy,
                    UpdatedAt = ur.User.UpdatedAt,
                    UpdatedBy = ur.User.UpdatedBy
                })
                .ToListAsync();

            return Result<List<UserDto>>.Success(users);
        }
    }
}
