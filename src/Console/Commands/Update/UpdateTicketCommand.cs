using System.ComponentModel;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Update;

/// <summary>
/// Settings for the 'update ticket' command.
/// </summary>
public class UpdateTicketCommand : UpdateCommand
{
    /// <summary>
    /// The key of the ticket to update.
    /// </summary>
    [CommandArgument(0, "[key]")]
    [Description("The key of the ticket to update.")]
    public string? Key { get; set; }

    /// <summary>
    /// The ID of the ticket to update.
    /// </summary>
    [CommandOption("-i|--id <id>")]
    [Description("The ID of the ticket to update.")]
    public long? Id { get; set; }

    /// <summary>
    /// The key of the status for this ticket.
    /// </summary>
    [CommandOption("-s|--status <key>")]
    [Description("The key of the status for this ticket.")]
    public string? StatusKey { get; set; }

    /// <summary>
    /// The ID of the status for this ticket.
    /// </summary>
    [CommandOption("--status-id <id>")]
    [Description("The ID of the status for this ticket.")]
    public long? StatusId { get; set; }

    /// <summary>
    /// The title of the ticket.
    /// </summary>
    [CommandOption("-t|--title <TITLE>")]
    [Description("The title of the ticket.")]
    public string? Title { get; set; }

    /// <summary>
    /// The description of the ticket.
    /// </summary>
    [CommandOption("-d|--description <DESCRIPTION>")]
    [Description("The description of the ticket.")]
    public string? Description { get; set; }
}
