using System.ComponentModel;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Delete;

/// <summary>
/// Settings for the 'delete ticket' command.
/// </summary>
public class DeleteTicketCommand : DeleteCommand
{
    /// <summary>
    /// The key of the ticket to delete.
    /// </summary>
    [CommandArgument(0, "[key]")]
    [Description("The key of the ticket to delete.")]
    public string? Key { get; set; }

    /// <summary>
    /// The ID of the ticket to delete.
    /// </summary>
    [CommandOption("-i|--id <id>")]
    [Description("The ID of the ticket to delete.")]
    public long? Id { get; set; }
}
