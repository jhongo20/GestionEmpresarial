using GestionEmpresarial.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionEmpresarial.Infrastructure.Persistence.Configurations
{
    public class RoleModuleConfiguration : IEntityTypeConfiguration<RoleModule>
    {
        public void Configure(EntityTypeBuilder<RoleModule> builder)
        {
            builder.HasKey(rm => rm.Id);

            builder.Property(rm => rm.IsActive)
                .IsRequired();

            builder.Property(rm => rm.IsDeleted)
                .IsRequired();

            // Relaciones
            builder.HasOne(rm => rm.Role)
                .WithMany(r => r.RoleModules)
                .HasForeignKey(rm => rm.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(rm => rm.Module)
                .WithMany(m => m.RoleModules)
                .HasForeignKey(rm => rm.ModuleId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
