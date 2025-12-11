using System.ComponentModel;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Update;

/// <summary>
/// Settings for the 'update iteration' command.
/// </summary>
public class UpdateIterationCommand : UpdateCommand
{
    /// <summary>
    /// The key of the iteration to update.
    /// </summary>
    [CommandArgument(0, "[key]")]
    [Description("The key of the iteration to update.")]
    public string? Key { get; set; }

    /// <summary>
    /// The ID of the iteration to update.
    /// </summary>
    [CommandOption("-i|--id <id>")]
    [Description("The ID of the iteration to update.")]
    public long? Id { get; set; }

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
