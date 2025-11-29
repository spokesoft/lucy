using Lucy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lucy.Infrastructure.Database.Configurations;

/// <summary>
/// Configuration for the Ticket entity.
/// </summary>
public class TicketTypeConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.ProjectId).IsRequired();
        builder.Property(t => t.StatusId).IsRequired();
        builder.Property(t => t.Key).IsRequired().HasMaxLength(20);
        builder.Property(t => t.Number).IsRequired();
        builder.Property(t => t.Title).IsRequired().HasMaxLength(200);
        builder.Property(t => t.Description).HasMaxLength(5000);

        builder.HasIndex(t => t.Key).IsUnique();
        builder.HasIndex(t => t.ProjectId);
        builder.HasIndex(t => t.StatusId);

        // Configure the relationship with Project
        builder.HasOne(t => t.Project)
            .WithMany(p => p.Tickets)
            .HasForeignKey(t => t.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure the relationship with Status
        builder.HasOne(t => t.Status)
            .WithMany(s => s.Tickets)
            .HasForeignKey(t => t.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        // Configure the relationship with Comments
        builder.HasMany(t => t.Comments)
            .WithOne(c => c.Ticket)
            .HasForeignKey(c => c.TicketId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();

        // Configure the relationship with TicketTags
        builder.HasMany(t => t.TicketTags)
            .WithOne(tt => tt.Ticket)
            .HasForeignKey(tt => tt.TicketId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}
