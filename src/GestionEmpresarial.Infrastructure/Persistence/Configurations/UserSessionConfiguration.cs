using GestionEmpresarial.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionEmpresarial.Infrastructure.Persistence.Configurations
{
    public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
    {
        public void Configure(EntityTypeBuilder<UserSession> builder)
        {
            builder.ToTable("UserSessions");

            builder.HasKey(us => us.Id);

            // Configuración de propiedades
            builder.Property(us => us.IpAddress)
                .HasMaxLength(50);

            builder.Property(us => us.UserAgent)
                .HasMaxLength(500);

            builder.Property(us => us.CreatedBy)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(us => us.UpdatedBy)
                .HasMaxLength(50);
        }
    }
}
