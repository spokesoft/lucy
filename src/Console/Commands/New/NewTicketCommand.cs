using System.ComponentModel;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.New;

/// <summary>
/// Settings for the 'new ticket' command.
/// </summary>
/// <remarks>
/// A project and status can be identified by either their keys or their IDs.
/// </remarks>
public class NewTicketCommand : NewCommand
{
    /// <summary>
    /// The title of the ticket.
    /// </summary>
    [CommandArgument(0, "<title>")]
    [Description("The title of the ticket.")]
    public required string Title { get; set; }

    /// <summary>
    /// The key of the project this ticket belongs to.
    /// </summary>
    [CommandOption("-p|--project <key>")]
    [Description("The key of the project this ticket belongs to.")]
    public string? ProjectKey { get; set; }

    /// <summary>
    /// The ID of the project this ticket belongs to.
    /// </summary>
    [CommandOption("--project-id <id>")]
    [Description("The ID of the project this ticket belongs to.")]
    public long? ProjectId { get; set; }

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
    /// The description of the ticket.
    /// </summary>
    [CommandOption("-d|--description <DESCRIPTION>")]
    [Description("The description of the ticket.")]
    public string? Description { get; set; }
}
