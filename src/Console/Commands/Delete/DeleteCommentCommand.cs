using System.ComponentModel;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Delete;

/// <summary>
/// Settings for the 'delete comment' command.
/// </summary>
public class DeleteCommentCommand : DeleteCommand
{
    /// <summary>
    /// The ID of the comment to delete.
    /// </summary>
    [CommandArgument(0, "<id>")]
    [Description("The ID of the comment to delete.")]
    public long Id { get; set; }
}
