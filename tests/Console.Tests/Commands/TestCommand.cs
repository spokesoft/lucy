using System.ComponentModel;
using Spectre.Console.Cli;

namespace Lucy.Console.Tests.Commands;

/// <summary>
/// Test command for unit testing purposes.
/// </summary>
public class TestCommand : Lucy.Console.Commands.Command
{
    /// <summary>
    /// Test property for validation.
    /// </summary>
    [CommandArgument(0, "<name>")]
    [Description("Test name argument.")]
    public required string Name { get; set; }

    /// <summary>
    /// Optional test property.
    /// </summary>
    [CommandOption("-v|--value <VALUE>")]
    [Description("Optional test value.")]
    public string? Value { get; set; }
}
