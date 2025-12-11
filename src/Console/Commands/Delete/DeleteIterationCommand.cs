using System.ComponentModel;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Delete;

/// <summary>
/// Settings for the 'delete iteration' command.
/// </summary>
/// <remarks>
/// An iteration can be identified by either its key or its ID. If both are
/// provided, the key is ignored.
/// </remarks>
public class DeleteIterationCommand : DeleteCommand
{
    /// <summary>
    /// The key of the iteration to delete.
    /// </summary>
    [CommandArgument(0, "[key]")]
    [Description("The key of the iteration to delete.")]
    public required string? Key { get; set; }

    /// <summary>
    /// The ID of the iteration to delete.
    /// </summary>
    [CommandOption("-i|--id <id>")]
    [Description("The ID of the iteration to delete.")]
    public required long? Id { get; set; }
}
