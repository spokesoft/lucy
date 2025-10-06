using Lucy.Infrastructure.Logging.Database.Converters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Lucy.Infrastructure.Logging.Database;

/// <summary>
/// Configuration for the log entry.
/// </summary>
public class LogEntryTypeConfiguration : IEntityTypeConfiguration<LogEntry>
{
    public void Configure(EntityTypeBuilder<LogEntry> builder)
    {
        builder.ToTable("logs");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.EventId)
            .HasConversion<EventIdConverter>()
            .IsRequired();
        builder.Property(e => e.Level)
            .HasConversion<LogLevelConverter>()
            .IsRequired();
        builder.Property(e => e.Category)
            .HasMaxLength(200)
            .IsRequired();
        builder.Property(e => e.Message)
            .HasMaxLength(1000)
            .IsRequired();
        builder.Property(e => e.Exception)
            .HasMaxLength(2000);
        builder.Property(e => e.Timestamp).IsRequired();
    }
}
