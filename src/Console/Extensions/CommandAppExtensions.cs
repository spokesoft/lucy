using System.Reflection;
using Spectre.Console.Cli;

namespace Lucy.Console.Extensions;

/// <summary>
/// Extension methods for <see cref="ICommandApp"/>.
/// </summary>
internal static class CommandAppExtensions
{
    /// <summary>
    /// Gets the version from the specified assembly.
    /// </summary>
    public static string GetVersionFromAssembly(this ICommandApp _, Assembly assembly)
    {
        var version = assembly
            .GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false)
            .OfType<AssemblyInformationalVersionAttribute>()
            .FirstOrDefault()?.InformationalVersion;

        return version ?? "0.0.0";
    }
}
