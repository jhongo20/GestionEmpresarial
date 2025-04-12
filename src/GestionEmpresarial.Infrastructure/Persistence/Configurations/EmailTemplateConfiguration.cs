using GestionEmpresarial.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GestionEmpresarial.Infrastructure.Persistence.Configurations
{
    public class EmailTemplateConfiguration : IEntityTypeConfiguration<EmailTemplate>
    {
        public void Configure(EntityTypeBuilder<EmailTemplate> builder)
        {
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Subject)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.HtmlBody)
                .IsRequired()
                .HasColumnType("nvarchar(max)");

            builder.Property(e => e.PlainTextBody)
                .HasColumnType("nvarchar(max)");

            builder.Property(e => e.Description)
                .HasMaxLength(500);

            builder.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.AvailableVariables)
                .HasColumnType("nvarchar(max)");

            builder.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(e => e.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(e => e.IsDefault)
                .IsRequired()
                .HasDefaultValue(false);

            builder.ToTable("EmailTemplates");
        }
    }
}
