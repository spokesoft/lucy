using System.ComponentModel;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Delete;

/// <summary>
/// Settings for the 'delete tag' command.
/// </summary>
/// <remarks>
/// A tag can be identified by either its key or its ID. If both are
/// provided, the key is ignored.
/// A project can be identified by either its key or its ID. If both are
/// provided, the key is ignored.
/// </remarks>
public class DeleteTagCommand : DeleteCommand
{
    /// <summary>
    /// The key of the project the tag belongs to.
    /// </summary>
    [CommandArgument(0, "<project-key>")]
    [Description("The key of the project the tag belongs to.")]
    public string? ProjectKey { get; set; }

    /// <summary>
    /// The key of the tag to delete.
    /// </summary>
    [CommandArgument(1, "[tag-key]")]
    [Description("The key of the tag to delete.")]
    public string? TagKey { get; set; }

    /// <summary>
    /// The ID of the project the tag belongs to.
    /// </summary>
    [CommandOption("-p|--project-id <id>")]
    [Description("The ID of the project the tag belongs to.")]
    public long? ProjectId { get; set; }

    /// <summary>
    /// The ID of the tag to delete.
    /// </summary>
    [CommandOption("-i|--id <id>")]
    [Description("The ID of the tag to delete.")]
    public long? TagId { get; set; }
}
