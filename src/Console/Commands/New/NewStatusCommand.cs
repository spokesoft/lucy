using System.ComponentModel;
using Lucy.Domain.Enums;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.New;

/// <summary>
/// Settings for the 'new status' command.
/// </summary>
/// <remarks>
/// A project can be identified by either its key or its ID. If both are
/// provided, the key is ignored.
/// </remarks>
public class NewStatusCommand : NewCommand
{
    /// <summary>
    /// The key of the project this status belongs to.
    /// </summary>
    [CommandArgument(0, "<project-key>")]
    [Description("The key of the project this status belongs to.")]
    public required string ProjectKey { get; set; }

    /// <summary>
    /// The unique key for the status.
    /// </summary>
    [CommandArgument(1, "<key>")]
    [Description("The unique key for the status.")]
    public required string Key { get; set; }

    /// <summary>
    /// The ID of the project this status belongs to.
    /// </summary>
    [CommandOption("-p|--project-id <id>")]
    [Description("The ID of the project this status belongs to.")]
    public required long? ProjectId { get; set; }

    /// <summary>
    /// The name of the status.
    /// </summary>
    [CommandOption("-n|--name <NAME>")]
    [Description("The name of the status.")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The description of the status.
    /// </summary>
    [CommandOption("-d|--description <DESCRIPTION>")]
    [Description("The description of the status.")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// The order of the status.
    /// </summary>
    [CommandOption("-o|--order <ORDER>")]
    [Description("The order of the status.")]
    public int? Order { get; set; }

    /// <summary>
    /// The color of the status.
    /// </summary>
    [CommandOption("-c|--color <COLOR>")]
    [Description("The color of the status.")]
    public Color? Color { get; set; }
}
