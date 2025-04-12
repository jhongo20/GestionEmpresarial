using GestionEmpresarial.Application.Common.Interfaces;
using GestionEmpresarial.Application.Common.Models;
using GestionEmpresarial.Domain.Entities;
using GestionEmpresarial.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GestionEmpresarial.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly IApplicationDbContext _context;
        private readonly IJwtGenerator _jwtGenerator;
        private readonly ILogger<IdentityService> _logger;
        private readonly IDateTime _dateTime;
        private readonly ILdapService _ldapService;

        public IdentityService(
            IApplicationDbContext context,
            IJwtGenerator jwtGenerator,
            ILogger<IdentityService> logger,
            IDateTime dateTime,
            ILdapService ldapService)
        {
            _context = context;
            _jwtGenerator = jwtGenerator;
            _logger = logger;
            _dateTime = dateTime;
            _ldapService = ldapService;
        }

        public async Task<Result<AuthenticationResponse>> AuthenticateAsync(string username, string password, string ipAddress)
        {
            try
            {
                _logger.LogInformation("Intentando autenticar al usuario {Username}", username);
                
                // Buscar usuario en la base de datos local
                var user = await _context.Users
                    .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                    .FirstOrDefaultAsync(u => u.Username == username && !u.IsDeleted);

                bool isAuthenticated = false;
                bool isLdapUser = false;

                // Si el usuario existe en la base de datos local
                if (user != null)
                {
                    // Si es un usuario LDAP, autenticar contra LDAP
                    if (user.IsLdapUser)
                    {
                        isAuthenticated = await _ldapService.AuthenticateAsync(username, password);
                        isLdapUser = true;
                    }
                    // Si no es un usuario LDAP, verificar contraseña localmente
                    else
                    {
                        isAuthenticated = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
                    }
                }
                // Si el usuario no existe en la base de datos local, intentar autenticar contra LDAP
                else if (_ldapService.IsEnabled())
                {
                    isAuthenticated = await _ldapService.AuthenticateAsync(username, password);
                    
                    // Si la autenticación LDAP es exitosa, crear el usuario en la base de datos local
                    if (isAuthenticated)
                    {
                        isLdapUser = true;
                        
                        // Obtener información del usuario desde LDAP
                        string email = await _ldapService.GetUserEmailAsync(username);
                        string displayName = await _ldapService.GetUserDisplayNameAsync(username);
                        
                        // Separar el nombre completo en nombre y apellido (si es posible)
                        string firstName = displayName;
                        string lastName = string.Empty;
                        
                        if (displayName.Contains(" "))
                        {
                            var nameParts = displayName.Split(' ', 2);
                            firstName = nameParts[0];
                            lastName = nameParts[1];
                        }
                        
                        // Crear el usuario en la base de datos local
                        user = new User
                        {
                            Id = Guid.NewGuid(),
                            Username = username,
                            Email = !string.IsNullOrEmpty(email) ? email : $"{username}@example.com",
                            FirstName = firstName,
                            LastName = lastName,
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword(Guid.NewGuid().ToString()), // Contraseña aleatoria
                            Status = UserStatus.Active,
                            UserType = UserType.LdapUser,
                            IsLdapUser = true,
                            IsEmailConfirmed = true, // Los usuarios LDAP ya tienen el correo confirmado
                            EmailConfirmedAt = _dateTime.Now,
                            CreatedBy = "System",
                            CreatedAt = _dateTime.Now,
                            UpdatedBy = "System",
                            UpdatedAt = _dateTime.Now
                        };
                        
                        await _context.Users.AddAsync(user);
                        
                        // Asignar el rol por defecto configurado en LdapSettings
                        var defaultRole = await _context.Roles
                            .FirstOrDefaultAsync(r => r.Name == _ldapService.GetDefaultRoleName());
                        
                        if (defaultRole != null)
                        {
                            var userRole = new UserRole
                            {
                                Id = Guid.NewGuid(),
                                UserId = user.Id,
                                RoleId = defaultRole.Id,
                                CreatedBy = "System",
                                CreatedAt = _dateTime.Now,
                                UpdatedBy = "System",
                                UpdatedAt = _dateTime.Now
                            };
                            
                            await _context.UserRoles.AddAsync(userRole);
                        }
                        
                        await _context.SaveChangesAsync(default);
                        
                        // Recargar el usuario con sus roles
                        user = await _context.Users
                            .Include(u => u.UserRoles)
                            .ThenInclude(ur => ur.Role)
                            .FirstOrDefaultAsync(u => u.Id == user.Id);
                    }
                }
                
                // Si la autenticación falló o el usuario está inactivo
                if (!isAuthenticated || user == null || user.Status != UserStatus.Active)
                {
                    _logger.LogWarning("Intento de autenticación fallido para el usuario {Username}", username);
                    return Result<AuthenticationResponse>.Failure("Usuario o contraseña incorrectos.");
                }
                
                // Actualizar fecha de último login
                user.LastLoginDate = _dateTime.Now;
                _context.Users.Update(user);
                await _context.SaveChangesAsync(default);
                
                // Obtener roles del usuario
                var roles = user.UserRoles
                    .Where(ur => !ur.IsDeleted)
                    .Select(ur => ur.Role.Name)
                    .ToList();
                
                // Generar token JWT
                var jwtResult = _jwtGenerator.GenerateJwtToken(
                    user.Id.ToString(),
                    user.Username,
                    user.Email,
                    roles);
                
                // Generar refresh token
                var refreshToken = _jwtGenerator.GenerateRefreshToken(ipAddress);
                
                // Guardar refresh token en la base de datos
                var userRefreshToken = new RefreshToken
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    Token = refreshToken.Token,
                    Expires = refreshToken.Expires,
                    Created = refreshToken.Created,
                    CreatedByIp = ipAddress,
                    CreatedBy = "System",
                    CreatedAt = _dateTime.Now,
                    UpdatedBy = "System",
                    UpdatedAt = _dateTime.Now
                };
                
                await _context.RefreshTokens.AddAsync(userRefreshToken);
                await _context.SaveChangesAsync(default);
                
                // Crear respuesta de autenticación
                var response = new AuthenticationResponse
                {
                    Id = user.Id.ToString(),
                    Username = user.Username,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Token = jwtResult.Token,
                    RefreshToken = refreshToken.Token,
                    Expiration = jwtResult.Expiration,
                    Roles = roles,
                    IsLdapUser = isLdapUser
                };
                
                _logger.LogInformation("Usuario {Username} autenticado correctamente", username);
                return Result<AuthenticationResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la autenticación del usuario {Username}", username);
                return Result<AuthenticationResponse>.Failure($"Error durante la autenticación: {ex.Message}");
            }
        }

        public async Task<Result<AuthenticationResponse>> RefreshTokenAsync(string token, string ipAddress)
        {
            try
            {
                // En un entorno real, verificaríamos el refresh token en la base de datos
                // Para simplificar, generamos un nuevo token
                
                var userId = Guid.NewGuid();
                var username = "admin";
                var roles = new List<string> { "Admin" };
                
                // Generar nuevo token JWT
                var jwtResult = _jwtGenerator.GenerateJwtToken(
                    userId.ToString(),
                    username,
                    "admin@gestionempresarial.com",
                    roles);
                
                // Generar nuevo refresh token
                var refreshToken = _jwtGenerator.GenerateRefreshToken(ipAddress);
                
                // Crear respuesta de autenticación
                var response = new AuthenticationResponse
                {
                    Id = userId.ToString(),
                    Username = username,
                    Email = "admin@gestionempresarial.com",
                    Token = jwtResult.Token,
                    RefreshToken = refreshToken.Token,
                    Expiration = jwtResult.Expiration,
                    Roles = roles
                };
                
                return Result<AuthenticationResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el refresh token");
                return Result<AuthenticationResponse>.Failure($"Error durante el refresh token: {ex.Message}");
            }
        }

        public async Task<Result> RevokeTokenAsync(string token, string ipAddress)
        {
            try
            {
                // En un entorno real, marcaríamos el token como revocado en la base de datos
                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la revocación del token");
                return Result.Failure($"Error durante la revocación del token: {ex.Message}");
            }
        }

        public async Task<Result<string>> RegisterUserAsync(string username, string email, string password)
        {
            try
            {
                // En un entorno real, crearíamos un nuevo usuario en la base de datos
                return Result<string>.Success(Guid.NewGuid().ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante el registro del usuario {Username}", username);
                return Result<string>.Failure($"Error durante el registro del usuario: {ex.Message}");
            }
        }
    }
}
