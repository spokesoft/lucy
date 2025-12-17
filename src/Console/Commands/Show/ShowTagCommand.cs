using System.ComponentModel;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Show;

/// <summary>
/// Settings for the 'show tag' command.
/// </summary>
/// <remarks>
/// A tag can be identified by either its key or its ID. If both are
/// provided, the key is ignored.
/// A project can be identified by either its key or its ID. If both are
/// provided, the key is ignored.
/// </remarks>
public class ShowTagCommand : ShowCommand
{
    /// <summary>
    /// The key of the project the tag belongs to.
    /// </summary>
    [CommandArgument(0, "[project-key]")]
    [Description("The key of the project the tag belongs to.")]
    public string? ProjectKey { get; set; }

    /// <summary>
    /// The key of the tag to show.
    /// </summary>
    [CommandArgument(1, "[tag-key]")]
    [Description("The key of the tag to show.")]
    public string? TagKey { get; set; }

    /// <summary>
    /// The ID of the project the tag belongs to.
    /// </summary>
    [CommandOption("-p|--project-id <id>")]
    [Description("The ID of the project the tag belongs to.")]
    public long? ProjectId { get; set; }

    /// <summary>
    /// The ID of the tag to show.
    /// </summary>
    [CommandOption("-i|--id <id>")]
    [Description("The ID of the tag to show.")]
    public long? TagId { get; set; }
}
