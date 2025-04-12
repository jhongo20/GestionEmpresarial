using GestionEmpresarial.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionEmpresarial.Infrastructure.Persistence.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.UserId)
                .HasMaxLength(50);

            builder.Property(e => e.UserName)
                .HasMaxLength(100);

            builder.Property(e => e.Type)
                .HasMaxLength(50);

            builder.Property(e => e.TableName)
                .HasMaxLength(100);

            builder.Property(e => e.DateTime)
                .IsRequired();

            builder.Property(e => e.OldValues)
                .HasColumnType("nvarchar(max)");

            builder.Property(e => e.NewValues)
                .HasColumnType("nvarchar(max)");

            builder.Property(e => e.AffectedColumns)
                .HasColumnType("nvarchar(max)");

            builder.Property(e => e.PrimaryKey)
                .HasMaxLength(100);

            builder.Property(e => e.Action)
                .HasMaxLength(50);

            builder.Property(e => e.IpAddress)
                .HasMaxLength(50);

            builder.Property(e => e.UserAgent)
                .HasMaxLength(500);

            builder.ToTable("AuditLogs");
        }
    }
}
