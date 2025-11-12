using Lucy.Domain.Entities;
using Lucy.Infrastructure.Database.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lucy.Infrastructure.Database.Configurations;

/// <summary>
/// Configuration for the Sequence entity.
/// </summary>
public class SequenceTypeConfiguration : IEntityTypeConfiguration<Sequence>
{
    public void Configure(EntityTypeBuilder<Sequence> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.ProjectId).IsRequired();
        builder.Property(s => s.Type)
            .HasConversion<SequenceTypeConverter>()
            .IsRequired();
        builder.Property(s => s.Value).IsRequired();
        builder.Property(s => s.Template).IsRequired().HasMaxLength(100);
        builder.Property(s => s.CreatedAt).IsRequired();
        builder.Property(s => s.UpdatedAt).IsRequired();

        builder.HasIndex(s => new { s.ProjectId, s.Type }).IsUnique();
    }
}
