using GestionEmpresarial.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionEmpresarial.Infrastructure.Persistence.Configurations
{
    public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
    {
        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            builder.ToTable("RolePermissions");

            builder.HasKey(rp => rp.Id);

            // Configuración de índices
            builder.HasIndex(rp => new { rp.RoleId, rp.PermissionId })
                .IsUnique();

            // Configuración de propiedades
            builder.Property(rp => rp.CreatedBy)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(rp => rp.UpdatedBy)
                .HasMaxLength(50);
        }
    }
}
