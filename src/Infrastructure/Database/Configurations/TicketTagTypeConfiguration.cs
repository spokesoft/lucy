using Lucy.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lucy.Infrastructure.Database.Configurations;

public class TicketTagTypeConfiguration : IEntityTypeConfiguration<TicketTag>
{
    public void Configure(EntityTypeBuilder<TicketTag> builder)
    {
        builder.HasKey(tt => tt.Id);

        builder.Property(tt => tt.TicketId).IsRequired();
        builder.Property(tt => tt.TagId).IsRequired();
        builder.Property(s => s.CreatedAt).IsRequired();
        builder.Property(s => s.UpdatedAt).IsRequired();

        builder.HasIndex(tt => new { tt.TicketId, tt.TagId }).IsUnique();
    }
}
