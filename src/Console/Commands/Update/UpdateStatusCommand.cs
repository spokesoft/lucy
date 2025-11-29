using System.ComponentModel;
using Lucy.Domain.Enums;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Update;

/// <summary>
/// Settings for the 'update status' command.
/// </summary>
/// <remarks>
/// A status can be identified by either its key or its ID. If both are
/// provided, the key is ignored.
/// A project can be identified by either its key or its ID. If both are
/// provided, the key is ignored.
/// </remarks>
public class UpdateStatusCommand : UpdateCommand
{
    /// <summary>
    /// The key of the project the status belongs to.
    /// </summary>
    [CommandArgument(0, "[project-key]")]
    [Description("The key of the project the status belongs to.")]
    public required string? ProjectKey { get; set; }

    /// <summary>
    /// The key of the status to update.
    /// </summary>
    [CommandArgument(1, "[status-key]")]
    [Description("The key of the status to update.")]
    public required string? StatusKey { get; set; }

    /// <summary>
    /// The ID of the project the status belongs to.
    /// </summary>
    [CommandOption("-p|--project-id <id>")]
    [Description("The ID of the project the status belongs to.")]
    public required long? ProjectId { get; set; }

    /// <summary>
    /// The ID of the status to update.
    /// </summary>
    [CommandOption("-i|--id <id>")]
    [Description("The ID of the status to update.")]
    public required long? StatusId { get; set; }

    /// <summary>
    /// New key for the status.
    /// </summary>
    [CommandOption("--new-key <KEY>")]
    [Description("New key for the status.")]
    public string? NewKey { get; set; }

    /// <summary>
    /// Order of the status.
    /// </summary>
    [CommandOption("-o|--order <ORDER>")]
    [Description("Order of the status.")]
    public int? Order { get; set; }

    /// <summary>
    /// Name of the status.
    /// </summary>
    [CommandOption("-n|--name <NAME>")]
    [Description("Name of the status.")]
    public string? Name { get; set; }

    /// <summary>
    /// Description of the status.
    /// </summary>
    [CommandOption("-d|--description <DESCRIPTION>")]
    [Description("Description of the status.")]
    public string? Description { get; set; }

    /// <summary>
    /// Color of the status.
    /// </summary>
    [CommandOption("-c|--color <COLOR>")]
    [Description("Color of the status.")]
    public Color? Color { get; set; }
}
