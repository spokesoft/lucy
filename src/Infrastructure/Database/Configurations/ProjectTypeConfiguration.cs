using Lucy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lucy.Infrastructure.Database.Configurations;

/// <summary>
/// Configuration for the Project entity.
/// </summary>
public class ProjectTypeConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Key).IsRequired().HasMaxLength(10);
        builder.Property(p => p.Name).HasMaxLength(100);
        builder.Property(p => p.Description).HasMaxLength(500);
        builder.Property(p => p.CreatedAt).IsRequired();
        builder.Property(p => p.UpdatedAt).IsRequired();

        // Configure the sequences relationship
        builder.HasMany(p => p.Sequences)
            .WithOne(s => s.Project)
            .HasForeignKey(s => s.ProjectId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        // Configure the statuses relationship
        builder.HasMany(p => p.Statuses)
            .WithOne()
            .HasForeignKey(s => s.ProjectId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        // Configure the tags relationship
        builder.HasMany(p => p.Tags)
            .WithOne(t => t.Project)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        // Configure the tickets relationship
        builder.HasMany(p => p.Tickets)
            .WithOne(t => t.Project)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        // Configure the comments relationship
        builder.HasMany(p => p.Comments)
            .WithOne(c => c.Project)
            .HasForeignKey(c => c.ProjectId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        // Configure the iterations relationship
        builder.HasMany(p => p.Iterations)
            .WithOne(i => i.Project)
            .HasForeignKey(i => i.ProjectId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        builder.HasIndex(p => p.Key).IsUnique();
    }
}
