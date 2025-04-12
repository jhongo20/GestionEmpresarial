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

        public IdentityService(
            IApplicationDbContext context,
            IJwtGenerator jwtGenerator,
            ILogger<IdentityService> logger,
            IDateTime dateTime)
        {
            _context = context;
            _jwtGenerator = jwtGenerator;
            _logger = logger;
            _dateTime = dateTime;
        }

        public async Task<Result<AuthenticationResponse>> AuthenticateAsync(string username, string password, string ipAddress)
        {
            try
            {
                _logger.LogInformation("Intentando autenticar al usuario {Username}", username);
                
                // Para simplificar, usamos credenciales hardcodeadas para pruebas
                // En un entorno real, verificaríamos contra la base de datos
                if (username == "admin" && password == "Admin123!")
                {
                    _logger.LogInformation("Usuario {Username} autenticado correctamente con credenciales hardcodeadas", username);
                    
                    var userId = Guid.NewGuid();
                    var roles = new List<string> { "Admin" };
                    
                    // Generar token JWT
                    var jwtResult = _jwtGenerator.GenerateJwtToken(
                        userId.ToString(),
                        username,
                        "admin@gestionempresarial.com",
                        roles);
                    
                    // Generar refresh token
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
                
                _logger.LogWarning("Intento de autenticación fallido para el usuario {Username}", username);
                return Result<AuthenticationResponse>.Failure("Usuario o contraseña incorrectos.");
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
