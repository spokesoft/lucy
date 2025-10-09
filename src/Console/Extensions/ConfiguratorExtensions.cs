using Lucy.Console.Internal;
using Microsoft.Extensions.Localization;
using Spectre.Console.Cli;

namespace Lucy.Console.Extensions;

/// <summary>
/// Extension methods for configuring CommandApp.
/// </summary>
internal static class ConfiguratorExtensions
{
    /// <summary>
    /// Adds an asynchronous delegate command with localized description and examples.
    /// </summary>
    public static void AddAsyncDelegate<T>(
        this IConfigurator<T> branch,
        string commandKey,
        ICommandExecutor executor,
        IStringLocalizer localizer)
        where T : CommandSettings
    {
        var contextKey = commandKey.Split('.').Last();
        var config = branch.AddAsyncDelegate<T>(localizer[$"Command.Context.{contextKey}"], executor.ExecuteAsync)
            .WithDescription(localizer[$"Command.{commandKey}.Description"]);

        localizer.GetAllStringsWhere(
            ls => ls.Name.StartsWith($"Command.{commandKey}.Example.", StringComparison.OrdinalIgnoreCase))
            .ToList()
            .ForEach(example => config.WithExample(
                example.Split(' ', StringSplitOptions.RemoveEmptyEntries)));
    }
}
