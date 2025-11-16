using Lucy.Domain.Entities;
using Lucy.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lucy.Infrastructure.Database.Configurations;

/// <summary>
/// Configuration for the Comment entity hierarchy (TPH).
/// </summary>
public class CommentTypeConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Content).IsRequired().HasMaxLength(5000);
        builder.Property(c => c.CreatedAt).IsRequired();
        builder.Property(c => c.UpdatedAt).IsRequired();

        // TPH Discriminator configuration
        builder.HasDiscriminator<CommentType>("CommentType")
            .HasValue<ProjectComment>(CommentType.Project)
            .HasValue<TicketComment>(CommentType.Ticket);
    }
}
