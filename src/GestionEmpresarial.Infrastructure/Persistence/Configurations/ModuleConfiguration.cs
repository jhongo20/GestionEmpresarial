using GestionEmpresarial.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionEmpresarial.Infrastructure.Persistence.Configurations
{
    public class ModuleConfiguration : IEntityTypeConfiguration<Module>
    {
        public void Configure(EntityTypeBuilder<Module> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.Description)
                .HasMaxLength(500);

            builder.Property(m => m.Icon)
                .HasMaxLength(50);

            builder.Property(m => m.Order)
                .IsRequired();

            builder.Property(m => m.IsActive)
                .IsRequired();

            builder.Property(m => m.IsDeleted)
                .IsRequired();

            // Relaciones
            builder.HasMany(m => m.Routes)
                .WithOne(r => r.Module)
                .HasForeignKey(r => r.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(m => m.RoleModules)
                .WithOne(rm => rm.Module)
                .HasForeignKey(rm => rm.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
