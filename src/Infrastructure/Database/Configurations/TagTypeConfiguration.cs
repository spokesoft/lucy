using Lucy.Domain.Entities;
using Lucy.Infrastructure.Database.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lucy.Infrastructure.Database.Configurations;

/// <summary>
/// Configuration for the Tag entity.
/// </summary>
public class TagTypeConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.HasKey(tag => tag.Id);

        builder.Property(s => s.ProjectId).IsRequired();
        builder.Property(s => s.Key).IsRequired().HasMaxLength(15);
        builder.Property(s => s.Label).HasMaxLength(50);
        builder.Property(s => s.Description).HasMaxLength(100);
        builder.Property(s => s.Color)
            .HasConversion<ColorConverter>()
            .IsRequired();

        builder.HasIndex(s => new { s.ProjectId, s.Key }).IsUnique();

        // Configure the relationship with TicketTags
        builder.HasMany(tag => tag.TicketTags)
            .WithOne(tt => tt.Tag)
            .HasForeignKey(tt => tt.TagId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}
