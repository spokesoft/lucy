using System.ComponentModel;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.List;

/// <summary>
/// Settings for the 'list tickets' command.
/// </summary>
public class ListTicketsCommand : ListCommand
{
    /// <summary>
    /// Gets or sets the project key.
    /// </summary>
    [CommandArgument(0, "[PROJECT_KEY]")]
    [Description("The key of the project to list tickets for.")]
    public string? Key { get; set; }

    /// <summary>
    /// Gets or sets the project ID.
    /// </summary>
    [CommandOption("--project-id")]
    [Description("The ID of the project to list tickets for.")]
    public long? Id { get; set; }

    /// <summary>
    /// Gets or sets the status key to filter by.
    /// </summary>
    [CommandOption("-s|--status")]
    [Description("The key of the status to filter tickets by.")]
    public string? StatusKey { get; set; }

    /// <summary>
    /// Gets or sets the status ID to filter by.
    /// </summary>
    [CommandOption("--status-id")]
    [Description("The ID of the status to filter tickets by.")]
    public long? StatusId { get; set; }

    /// <summary>
    /// Gets or sets the tag key to filter by.
    /// </summary>
    [CommandOption("-t|--tag")]
    [Description("The key of the tag to filter tickets by.")]
    public string? TagKey { get; set; }

    /// <summary>
    /// Gets or sets the tag ID to filter by.
    /// </summary>
    [CommandOption("--tag-id")]
    [Description("The ID of the tag to filter tickets by.")]
    public long? TagId { get; set; }
}
