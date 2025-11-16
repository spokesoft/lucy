using System.ComponentModel;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Show;

/// <summary>
/// Settings for the 'show ticket' command.
/// </summary>
public class ShowTicketCommand : ShowCommand
{
    /// <summary>
    /// The ID of the ticket to show.
    /// </summary>
    [CommandArgument(0, "<id>")]
    [Description("The ID of the ticket to show.")]
    public required long TicketId { get; set; }
}
