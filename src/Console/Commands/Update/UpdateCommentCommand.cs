using System.ComponentModel;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Update;

/// <summary>
/// Settings for the 'update comment' command.
/// </summary>
public class UpdateCommentCommand : UpdateCommand
{
    /// <summary>
    /// The ID of the comment to update.
    /// </summary>
    [CommandArgument(0, "<id>")]
    [Description("The ID of the comment to update.")]
    public long Id { get; set; }

    /// <summary>
    /// The new content for the comment.
    /// </summary>
    [CommandOption("-c|--content <CONTENT>")]
    [Description("The new content for the comment.")]
    public string? Content { get; set; }
}
