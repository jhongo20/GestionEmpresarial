using GestionEmpresarial.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionEmpresarial.Infrastructure.Persistence.Configurations
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("UserRoles");

            builder.HasKey(ur => ur.Id);

            // Configuración de índices
            builder.HasIndex(ur => new { ur.UserId, ur.RoleId })
                .IsUnique();

            // Configuración de propiedades
            builder.Property(ur => ur.CreatedBy)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(ur => ur.UpdatedBy)
                .HasMaxLength(50);
        }
    }
}
