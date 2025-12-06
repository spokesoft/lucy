using System.ComponentModel;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Remove;

/// <summary>
/// Settings for the 'remove tag' command (detach a tag from a ticket).
/// </summary>
/// <remarks>
/// Identify the ticket by key or --ticket-id, and the tag by key or --tag-id.
/// When using tag key, the ticket's project is used to resolve the tag.
/// </remarks>
public class RemoveTagCommand : RemoveCommand
{
    /// <summary>
    /// The key of the ticket to update (e.g., PROJ-123).
    /// </summary>
    [CommandArgument(0, "[ticket-key]")]
    [Description("The key of the ticket to remove the tag from.")]
    public string? TicketKey { get; set; }

    /// <summary>
    /// The ID of the ticket to update.
    /// </summary>
    [CommandOption("-i|--ticket-id <id>")]
    [Description("The ID of the ticket to remove the tag from.")]
    public long? TicketId { get; set; }

    /// <summary>
    /// The key of the tag to remove.
    /// </summary>
    [CommandArgument(1, "[tag-key]")]
    [Description("The key of the tag to remove.")]
    public string? TagKey { get; set; }

    /// <summary>
    /// The ID of the tag to remove.
    /// </summary>
    [CommandOption("-t|--tag-id <id>")]
    [Description("The ID of the tag to remove.")]
    public long? TagId { get; set; }
}
