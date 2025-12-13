using System.ComponentModel;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.List;

/// <summary>
/// Settings for the 'list iterations' command.
/// </summary>
public class ListIterationsCommand : ListCommand
{
    /// <summary>
    /// The project key to list iterations for.
    /// </summary>
    [CommandArgument(0, "[KEY]")]
    [Description("The project key to list iterations for.")]
    public string? ProjectKey { get; set; }

    /// <summary>
    /// The project ID to list iterations for.
    /// </summary>
    [CommandOption("--project-id <ID>")]
    [Description("The project ID to list iterations for.")]
    public long? ProjectId { get; set; }
}
