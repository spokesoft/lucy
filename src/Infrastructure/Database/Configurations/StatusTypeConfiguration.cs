using Lucy.Domain.Entities;
using Lucy.Infrastructure.Database.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lucy.Infrastructure.Database.Configurations;

/// <summary>
/// Configuration for the Status entity.
/// </summary>
public class StatusTypeConfiguration : IEntityTypeConfiguration<Status>
{
    public void Configure(EntityTypeBuilder<Status> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.ProjectId).IsRequired();
        builder.Property(s => s.Key).IsRequired().HasMaxLength(15);
        builder.Property(s => s.Order).IsRequired();
        builder.Property(s => s.Name).HasMaxLength(50);
        builder.Property(s => s.Description).HasMaxLength(100);
        builder.Property(s => s.Color)
            .HasConversion<StatusColorConverter>()
            .IsRequired();

        builder.HasIndex(s => new { s.ProjectId, s.Key }).IsUnique();

        // Configure the relationship with Project
        builder.HasOne(s => s.Project)
            .WithMany(p => p.Statuses)
            .HasForeignKey(s => s.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure the relationship with Tickets
        builder.HasMany(s => s.Tickets)
            .WithOne(t => t.Status)
            .HasForeignKey(t => t.StatusId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
