using Lucy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lucy.Infrastructure.Database.Configurations;

/// <summary>
/// Configuration for the TicketComment entity.
/// </summary>
public class TicketCommentTypeConfiguration : IEntityTypeConfiguration<TicketComment>
{
    public void Configure(EntityTypeBuilder<TicketComment> builder)
    {
        builder.HasIndex(c => c.TicketId);
    }
}
