using Lucy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lucy.Infrastructure.Database.Configurations;

/// <summary>
/// Configuration for the Iteration entity.
/// </summary>
public class IterationTypeConfiguration : IEntityTypeConfiguration<Iteration>
{
    public void Configure(EntityTypeBuilder<Iteration> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Key)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(i => i.ProjectId).IsRequired();
        builder.Property(i => i.Number).IsRequired();
        builder.Property(i => i.Name).HasMaxLength(100);
        builder.Property(i => i.Description).HasMaxLength(500);
        builder.Property(i => i.StartDate);
        builder.Property(i => i.EndDate);

        // Configure the ticket relationship
        builder.HasMany(i => i.Tickets)
            .WithOne(t => t.Iteration)
            .HasForeignKey(t => t.IterationId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => p.Key).IsUnique();
    }
}
