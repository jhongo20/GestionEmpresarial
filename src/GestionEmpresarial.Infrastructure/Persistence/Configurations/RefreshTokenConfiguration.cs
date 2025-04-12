using GestionEmpresarial.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionEmpresarial.Infrastructure.Persistence.Configurations
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("RefreshTokens");

            builder.HasKey(rt => rt.Id);

            // Configuración de propiedades
            builder.Property(rt => rt.Token)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(rt => rt.CreatedByIp)
                .HasMaxLength(50);

            builder.Property(rt => rt.RevokedByIp)
                .HasMaxLength(50);

            builder.Property(rt => rt.ReplacedByToken)
                .HasMaxLength(255);

            builder.Property(rt => rt.ReasonRevoked)
                .HasMaxLength(255);

            builder.Property(rt => rt.CreatedBy)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(rt => rt.UpdatedBy)
                .HasMaxLength(50);
        }
    }
}
