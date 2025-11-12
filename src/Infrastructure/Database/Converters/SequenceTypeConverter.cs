using Lucy.Domain.Enums;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Lucy.Infrastructure.Database.Converters;

/// <summary>
/// Converts SequenceType to and from a string representation.
/// </summary>
public class SequenceTypeConverter : ValueConverter<SequenceType, string>
{
    public SequenceTypeConverter() : base(
        v => ToProvider(v),
        v => FromProvider(v))
    {
    }

    /// <summary>
    /// Converts a SequenceType to its string representation.
    /// </summary>
    public static string ToProvider(SequenceType type) => type.ToString();

    /// <summary>
    /// Converts a string back to a SequenceType.
    /// </summary>
    public static SequenceType FromProvider(string value)
    {
        return Enum.TryParse<SequenceType>(value, out var type) ? type : SequenceType.None;
    }
}
