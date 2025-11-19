using System.ComponentModel;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Show;

/// <summary>
/// Settings for the 'show status' command.
/// </summary>
/// <remarks>
/// A status can be identified by either its key or its ID. If both are
/// provided, the key is ignored.
/// A project can be identified by either its key or its ID. If both are
/// provided, the key is ignored.
/// </remarks>
public class ShowStatusCommand : ShowCommand
{
    /// <summary>
    /// Whether to include tickets in the status view.
    /// </summary>
    [CommandOption("--include-tickets")]
    [Description("Include tickets for this status in the output.")]
    public bool IncludeTickets { get; set; }

    /// <summary>
    /// The key of the project the status belongs to.
    /// </summary>
    [CommandArgument(0, "[project-key]")]
    [Description("The key of the project the status belongs to.")]
    public required string? ProjectKey { get; set; }

    /// <summary>
    /// The key of the status to show.
    /// </summary>
    [CommandArgument(1, "[status-key]")]
    [Description("The key of the status to show.")]
    public required string? StatusKey { get; set; }

    /// <summary>
    /// The ID of the project the status belongs to.
    /// </summary>
    [CommandOption("-p|--project-id <id>")]
    [Description("The ID of the project the status belongs to.")]
    public required long? ProjectId { get; set; }

    /// <summary>
    /// The ID of the status to show.
    /// </summary>
    [CommandOption("-i|--id <id>")]
    [Description("The ID of the status to show.")]
    public required long? StatusId { get; set; }
}
