using GestionEmpresarial.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace GestionEmpresarial.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; }
        DbSet<Role> Roles { get; }
        DbSet<Permission> Permissions { get; }
        DbSet<UserRole> UserRoles { get; }
        DbSet<RolePermission> RolePermissions { get; }
        DbSet<RefreshToken> RefreshTokens { get; }
        DbSet<UserSession> UserSessions { get; }
        DbSet<ActivationToken> ActivationTokens { get; }
        DbSet<GestionEmpresarial.Domain.Entities.Module> Modules { get; }
        DbSet<Route> Routes { get; }
        DbSet<RoleModule> RoleModules { get; }
        DbSet<RoleRoute> RoleRoutes { get; }
        DbSet<AuditLog> AuditLogs { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
