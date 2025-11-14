using System.ComponentModel;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.List;

/// <summary>
/// Settings for the 'list statuses' command.
/// </summary>
public class ListStatusesCommand : ListCommand
{
    /// <summary>
    /// Gets or sets the project key.
    /// </summary>
    [CommandArgument(0, "[PROJECT_KEY]")]
    [Description("The key of the project to list statuses for.")]
    public string? Key { get; set; }

    /// <summary>
    /// Gets or sets the project ID.
    /// </summary>
    [CommandOption("--project-id")]
    [Description("The ID of the project to list statuses for.")]
    public long? Id { get; set; }
}
