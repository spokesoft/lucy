using Lucy.Domain.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Lucy.Infrastructure.Database.Converters;

/// <summary>
/// Converts StatusColor to and from a string representation.
/// </summary>
public class StatusColorConverter : ValueConverter<StatusColor, string>
{
    public StatusColorConverter() : base(
        v => ToProvider(v),
        v => FromProvider(v))
    {
    }

    /// <summary>
    /// Converts a StatusColor to its string representation.
    /// </summary>
    public static string ToProvider(StatusColor color) => color.ToString();

    /// <summary>
    /// Converts a string back to a StatusColor.
    /// </summary>
    public static StatusColor FromProvider(string value)
    {
        return Enum.TryParse<StatusColor>(value, out var color) ? color : StatusColor.Gray;
    }
}
