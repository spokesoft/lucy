using System.ComponentModel;
using Lucy.Domain.Enums;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Update;

/// <summary>
/// Settings for the 'update tag' command.
/// </summary>
/// <remarks>
/// A tag can be identified by either its key or its ID. If both are
/// provided, the key is ignored.
/// A project can be identified by either its key or its ID. If both are
/// provided, the key is ignored.
/// </remarks>
public class UpdateTagCommand : UpdateCommand
{
    /// <summary>
    /// The key of the project the tag belongs to.
    /// </summary>
    [CommandArgument(0, "[project-key]")]
    [Description("The key of the project the tag belongs to.")]
    public string? ProjectKey { get; set; }

    /// <summary>
    /// The key of the tag to update.
    /// </summary>
    [CommandArgument(1, "[tag-key]")]
    [Description("The key of the tag to update.")]
    public string? TagKey { get; set; }

    /// <summary>
    /// The ID of the project the tag belongs to.
    /// </summary>
    [CommandOption("-p|--project-id <id>")]
    [Description("The ID of the project the tag belongs to.")]
    public long? ProjectId { get; set; }

    /// <summary>
    /// The ID of the tag to update.
    /// </summary>
    [CommandOption("-i|--id <id>")]
    [Description("The ID of the tag to update.")]
    public long? TagId { get; set; }

    /// <summary>
    /// New key for the tag.
    /// </summary>
    [CommandOption("--new-key <KEY>")]
    [Description("New key for the tag.")]
    public string? NewKey { get; set; }

    /// <summary>
    /// Label of the tag.
    /// </summary>
    [CommandOption("-l|--label <LABEL>")]
    [Description("Label of the tag.")]
    public string? Label { get; set; }

    /// <summary>
    /// Description of the tag.
    /// </summary>
    [CommandOption("-d|--description <DESCRIPTION>")]
    [Description("Description of the tag.")]
    public string? Description { get; set; }

    /// <summary>
    /// Color of the tag.
    /// </summary>
    [CommandOption("-c|--color <COLOR>")]
    [Description("Color of the tag.")]
    public Color? Color { get; set; }
}
