using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using GestionEmpresarial.Application.Common.Interfaces;
using GestionEmpresarial.Application.Common.Models;
using GestionEmpresarial.Domain.Entities;
using GestionEmpresarial.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace GestionEmpresarial.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly IApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILdapService _ldapService;

        public IdentityService(IApplicationDbContext context, IConfiguration configuration, ILdapService ldapService)
        {
            _context = context;
            _configuration = configuration;
            _ldapService = ldapService;
        }

        public async Task<AuthenticationResponse> AuthenticateAsync(string username, string password)
        {
            var user = await _context.Users
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == username && !u.IsDeleted);

            if (user == null)
            {
                return null;
            }

            bool isAuthenticated = false;

            if (user.IsLdapUser)
            {
                // Si es un usuario LDAP, verificamos las credenciales contra el directorio activo
                isAuthenticated = await _ldapService.AuthenticateAsync(username, password);
            }
            else
            {
                // Si es un usuario local, verificamos el hash de la contraseña
                isAuthenticated = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
            }

            if (!isAuthenticated)
            {
                return null;
            }

            // Generar token JWT
            var token = GenerateJwtToken(user);

            return new AuthenticationResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsLdapUser = user.IsLdapUser,
                Token = token,
                Roles = user.UserRoles.Select(ur => ur.Role.Name).ToList()
            };
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["JwtSettings:Secret"]);

            var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
                new Claim("IsLdapUser", user.IsLdapUser.ToString())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
