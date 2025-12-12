using System.ComponentModel;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Remove;

/// <summary>
/// Settings for the 'remove ticket' command (unassign a ticket from an iteration).
/// </summary>
public class RemoveTicketCommand : RemoveCommand
{
    /// <summary>
    /// The key of the ticket.
    /// </summary>
    [CommandArgument(0, "[ticket-key]")]
    [Description("The key of the ticket.")]
    public string? TicketKey { get; set; }

    /// <summary>
    /// The ID of the ticket.
    /// </summary>
    [CommandOption("-t|--ticket-id <id>")]
    [Description("The ID of the ticket.")]
    public long? TicketId { get; set; }

    /// <summary>
    /// The key of the iteration.
    /// </summary>
    [CommandArgument(1, "[iteration-key]")]
    [Description("The key of the iteration.")]
    public string? IterationKey { get; set; }

    /// <summary>
    /// The ID of the iteration.
    /// </summary>
    [CommandOption("-i|--iteration-id <id>")]
    [Description("The ID of the iteration.")]
    public long? IterationId { get; set; }
}
