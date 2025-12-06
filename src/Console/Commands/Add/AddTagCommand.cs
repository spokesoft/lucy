using System.ComponentModel;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Add;

/// <summary>
/// Settings for the 'add tag' command (attach a tag to a ticket).
/// </summary>
/// <remarks>
/// You can identify the ticket by key or --ticket-id, and the tag by key or --tag-id.
/// When using tag key, the ticket is used to resolve the project for that tag.
/// </remarks>
public class AddTagCommand : AddCommand
{
    /// <summary>
    /// The key of the ticket to tag (e.g., PROJ-123).
    /// </summary>
    [CommandArgument(0, "[ticket-key]")]
    [Description("The key of the ticket to tag.")]
    public string? TicketKey { get; set; }

    /// <summary>
    /// The ID of the ticket to tag.
    /// </summary>
    [CommandOption("-i|--ticket-id <id>")]
    [Description("The ID of the ticket to tag.")]
    public long? TicketId { get; set; }

    /// <summary>
    /// The key of the tag to add to the ticket.
    /// </summary>
    [CommandArgument(1, "[tag-key]")]
    [Description("The key of the tag to add to the ticket.")]
    public string? TagKey { get; set; }

    /// <summary>
    /// The ID of the tag to add to the ticket.
    /// </summary>
    [CommandOption("-t|--tag-id <id>")]
    [Description("The ID of the tag to add to the ticket.")]
    public long? TagId { get; set; }
}
