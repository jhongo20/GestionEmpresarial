using GestionEmpresarial.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionEmpresarial.Infrastructure.Persistence.Configurations
{
    public class ActivationTokenConfiguration : IEntityTypeConfiguration<ActivationToken>
    {
        public void Configure(EntityTypeBuilder<ActivationToken> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Token)
                .IsRequired()
                .HasMaxLength(128);

            builder.Property(e => e.ExpiryDate)
                .IsRequired();

            builder.Property(e => e.IsUsed)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(e => e.UsedAt)
                .IsRequired(false);

            // Relación con User
            builder.HasOne(e => e.User)
                .WithMany(u => u.ActivationTokens)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Propiedades de auditoría
            builder.Property(e => e.CreatedBy)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.CreatedAt)
                .IsRequired();

            builder.Property(e => e.UpdatedBy)
                .HasMaxLength(50);
        }
    }
}
