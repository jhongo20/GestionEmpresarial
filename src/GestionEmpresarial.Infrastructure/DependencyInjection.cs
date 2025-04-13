using GestionEmpresarial.Application.Common.Interfaces;
using GestionEmpresarial.Application.Common.Models;
using GestionEmpresarial.Infrastructure.Identity;
using GestionEmpresarial.Infrastructure.Persistence;
using GestionEmpresarial.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Caching.Memory;

namespace GestionEmpresarial.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Configuración de la base de datos
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

            // Servicios de infraestructura
            services.AddTransient<IDateTime, DateTimeService>();
            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<IJwtGenerator, JwtGenerator>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<IPermissionService, PermissionService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IEmailService, EmailService>();
            
            // Servicios para módulos, rutas y menús
            services.AddScoped<IModuleService, ModuleService>();
            services.AddScoped<IRouteService, RouteService>();
            services.AddScoped<IMenuService, MenuService>();
            services.AddScoped<IAuditService, AuditService>();
            services.AddScoped<IEmailTemplateService, EmailTemplateService>();

            // Configuración del servicio de caché en memoria
            services.AddMemoryCache(options =>
            {
                // Configurar opciones de caché
                options.SizeLimit = 1024; // Tamaño máximo en MB (ajustar según necesidades)
                options.ExpirationScanFrequency = TimeSpan.FromMinutes(5); // Frecuencia de escaneo para eliminar entradas expiradas
            });

            // Configuración y registro del servicio LDAP
            services.Configure<LdapSettings>(configuration.GetSection("LdapSettings"));
            services.AddTransient<ILdapService, LdapService>();

            // Configuración de autenticación JWT
            var jwtSettings = configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true
                };
            });

            return services;
        }
    }
}
