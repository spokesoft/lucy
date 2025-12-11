using System.ComponentModel;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.New;

/// <summary>
/// Settings for the 'new iteration' command.
/// </summary>
/// <remarks>
/// A project can be identified by either its key or its ID.
/// </remarks>
public class NewIterationCommand : NewCommand
{
    /// <summary>
    /// The key of the project this iteration belongs to.
    /// </summary>
    [CommandOption("-p|--project <key>")]
    [Description("The key of the project this iteration belongs to.")]
    public string? ProjectKey { get; set; }

    /// <summary>
    /// The ID of the project this iteration belongs to.
    /// </summary>
    [CommandOption("--project-id <id>")]
    [Description("The ID of the project this iteration belongs to.")]
    public long? ProjectId { get; set; }

    /// <summary>
    /// The name of the iteration.
    /// </summary>
    [CommandOption("-n|--name <NAME>")]
    [Description("The name of the iteration.")]
    public string? Name { get; set; }

    /// <summary>
    /// The description of the iteration.
    /// </summary>
    [CommandOption("-d|--description <DESCRIPTION>")]
    [Description("The description of the iteration.")]
    public string? Description { get; set; }

    /// <summary>
    /// The start date of the iteration.
    /// </summary>
    [CommandOption("--start <DATE>")]
    [Description("The start date of the iteration (e.g., 2025-01-01).")]
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// The end date of the iteration.
    /// </summary>
    [CommandOption("--end <DATE>")]
    [Description("The end date of the iteration (e.g., 2025-01-31).")]
    public DateTime? EndDate { get; set; }
}
