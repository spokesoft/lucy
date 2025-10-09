using System.Globalization;
using Lucy.Console.Internal;
using Microsoft.Extensions.Localization;

namespace Lucy.Console.Extensions;

/// <summary>
/// Extension methods for IStringLocalizer.
/// </summary>
internal static class StringLocalizerExtensions
{
    /// <summary>
    /// Gets all examples for a given command from the localizer.
    /// </summary>
    public static IEnumerable<string> GetAllStringsWhere(this IStringLocalizer localizer, Func<LocalizedString, bool> predicate)
    {
        return localizer.GetAllStrings(true)
            .Where(ls => predicate(ls))
            .OrderBy(s => s.Name) // Ensure consistent order
            .Select(s => s.Value);
    }
}
