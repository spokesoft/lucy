using System.ComponentModel;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Show;

/// <summary>
/// Settings for the 'show iteration' command.
/// </summary>
public class ShowIterationCommand : ShowCommand
{
    /// <summary>
    /// The key of the iteration to show.
    /// </summary>
    [CommandArgument(0, "[key]")]
    [Description("The key of the iteration to show.")]
    public required string? Key { get; set; }

    /// <summary>
    /// The ID of the iteration to show.
    /// </summary>
    [CommandOption("-i|--id <id>")]
    [Description("The ID of the iteration to show.")]
    public required long? Id { get; set; }
}
