using System.ComponentModel;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Show;

/// <summary>
/// Settings for the 'show ticket' command.
/// </summary>
public class ShowTicketCommand : ShowCommand
{
    /// <summary>
    /// The key of the ticket to show.
    /// </summary>
    [CommandArgument(0, "[key]")]
    [Description("The key of the ticket to show.")]
    public string? Key { get; set; }

    /// <summary>
    /// The ID of the ticket to show.
    /// </summary>
    [CommandOption("-i|--id <id>")]
    [Description("The ID of the ticket to show.")]
    public long? Id { get; set; }
}
