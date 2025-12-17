using Lucy.Application.Common.Interfaces;
using Lucy.Application.Projects.Queries.ProjectExistsByKey;
using Lucy.Application.Common.Validation;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.New;

/// <summary>
/// Validator for the <see cref="NewProjectCommand"/> command.
/// </summary>
internal class NewProjectCommandValidator(
    IMediator mediator) : ICommandValidator<NewProjectCommand>
{
    /// <summary>
    /// The mediator instance for sending queries.
    /// </summary>
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ValidationResult> ValidateAsync(
        CommandContext context,
        NewProjectCommand command,
        CancellationToken token = default)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(command.Key))
        {
            result.AddError(
                ConsoleValidationCode.ProjectKeyRequired,
                nameof(command.Key));
            return result;
        }

        var exists = await _mediator.Send(new ProjectExistsByKeyQuery(command.Key), token);
        if (exists)
        {
            result.AddError(
                ConsoleValidationCode.ProjectAlreadyExists,
                nameof(command.Key),
                command.Key);
        }

        return result;
    }
}
