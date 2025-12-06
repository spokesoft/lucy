using System.ComponentModel;
using Lucy.Domain.Enums;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.New;

/// <summary>
/// Settings for the 'new tag' command.
/// </summary>
/// <remarks>
/// A project can be identified by either its key or its ID. If both are
/// provided, the key is ignored.
/// </remarks>
public class NewTagCommand : NewCommand
{
    /// <summary>
    /// The unique key for the tag.
    /// </summary>
    [CommandArgument(1, "<key>")]
    [Description("The unique key for the tag.")]
    public required string Key { get; set; }

    /// <summary>
    /// The key of the project this tag belongs to.
    /// </summary>
    [CommandOption("-p|--project <key>")]
    [Description("The key of the project this tag belongs to.")]
    public string? ProjectKey { get; set; }

    /// <summary>
    /// The ID of the project this tag belongs to.
    /// </summary>
    [CommandOption("--project-id <id>")]
    [Description("The ID of the project this tag belongs to.")]
    public long? ProjectId { get; set; }

    /// <summary>
    /// The label of the tag.
    /// </summary>
    [CommandOption("-l|--label <LABEL>")]
    [Description("The label of the tag.")]
    public string? Label { get; set; }

    /// <summary>
    /// The description of the tag.
    /// </summary>
    [CommandOption("-d|--description <DESCRIPTION>")]
    [Description("The description of the tag.")]
    public string? Description { get; set; }

    /// <summary>
    /// The color of the tag.
    /// </summary>
    [CommandOption("-c|--color <COLOR>")]
    [Description("The color of the tag.")]
    public Color? Color { get; set; }
}
