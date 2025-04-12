using GestionEmpresarial.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionEmpresarial.Infrastructure.Persistence.Configurations
{
    public class RoleRouteConfiguration : IEntityTypeConfiguration<RoleRoute>
    {
        public void Configure(EntityTypeBuilder<RoleRoute> builder)
        {
            builder.HasKey(rr => rr.Id);

            builder.Property(rr => rr.IsActive)
                .IsRequired();

            builder.Property(rr => rr.IsDeleted)
                .IsRequired();

            // Relaciones
            builder.HasOne(rr => rr.Role)
                .WithMany(r => r.RoleRoutes)
                .HasForeignKey(rr => rr.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(rr => rr.Route)
                .WithMany(r => r.RoleRoutes)
                .HasForeignKey(rr => rr.RouteId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
