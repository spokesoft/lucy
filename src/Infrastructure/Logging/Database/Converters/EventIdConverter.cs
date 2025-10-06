using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Logging;

namespace Lucy.Infrastructure.Logging.Database.Converters;

/// <summary>
/// Converts EventId to and from a serialized string representation.
/// </summary>
public class EventIdConverter : ValueConverter<EventId, string>
{
    public EventIdConverter() : base(
        v => ToProvider(v),
        v => FromProvider(v))
    {
    }

    /// <summary>
    /// Converts an EventId to a serialized string.
    /// </summary>
    public static string ToProvider(EventId eventId) => $"{eventId.Id}:{eventId.Name}";

    /// <summary>
    /// Converts a serialized string back to an EventId.
    /// </summary>
    public static EventId FromProvider(string value)
    {
        var parts = value.Split(':', 2);
        if (parts.Length == 2 && int.TryParse(parts[0], out var id))
        {
            return new EventId(id, parts[1]);
        }
        return new EventId(0, value);
    }
}
