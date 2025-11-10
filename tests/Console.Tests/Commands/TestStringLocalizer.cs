using Microsoft.Extensions.Localization;
using System.Globalization;

namespace Lucy.Console.Tests.Commands;

/// <summary>
/// Test string localizer for unit testing purposes.
/// </summary>
public class TestStringLocalizer<T> : IStringLocalizer<T>
{
    public LocalizedString this[string name] => new(name, name);

    public LocalizedString this[string name, params object[] arguments] => new(name, string.Format(name, arguments));

    public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
    {
        return Enumerable.Empty<LocalizedString>();
    }
}
