using System.ComponentModel;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Show;

/// <summary>
/// Settings for the 'show board' command.
/// </summary>
public class ShowBoardCommand : ShowCommand
{
    /// <summary>
    /// The key of the project to show the board for.
    /// </summary>
    [CommandArgument(0, "[key]")]
    [Description("The key of the project to show the board for.")]
    public required string? Key { get; set; }

    /// <summary>
    /// The ID of the project to show the board for.
    /// </summary>
    [CommandOption("-i|--id <id>")]
    [Description("The ID of the project to show the board for.")]
    public required long? Id { get; set; }
}
