using GestionEmpresarial.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionEmpresarial.Infrastructure.Persistence.Configurations
{
    public class RouteConfiguration : IEntityTypeConfiguration<Route>
    {
        public void Configure(EntityTypeBuilder<Route> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(r => r.Path)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(r => r.Icon)
                .HasMaxLength(50);

            builder.Property(r => r.Order)
                .IsRequired();

            builder.Property(r => r.IsActive)
                .IsRequired();

            builder.Property(r => r.IsDeleted)
                .IsRequired();

            // Relaciones
            builder.HasOne(r => r.Module)
                .WithMany(m => m.Routes)
                .HasForeignKey(r => r.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(r => r.RoleRoutes)
                .WithOne(rr => rr.Route)
                .HasForeignKey(rr => rr.RouteId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
