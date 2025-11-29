using Lucy.Domain.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Lucy.Infrastructure.Database.Converters;

/// <summary>
/// Converts StatusColor to and from a string representation.
/// </summary>
public class ColorConverter : ValueConverter<Color, string>
{
    public ColorConverter() : base(
        v => ToProvider(v),
        v => FromProvider(v))
    {
    }

    /// <summary>
    /// Converts a StatusColor to its string representation.
    /// </summary>
    public static string ToProvider(Color color) => color.ToString();

    /// <summary>
    /// Converts a string back to a StatusColor.
    /// </summary>
    public static Color FromProvider(string value)
    {
        return Enum.TryParse<Color>(value, out var color) ? color : Color.Gray;
    }
}
