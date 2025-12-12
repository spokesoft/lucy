using Lucy.Application.Interfaces;

using Lucy.Application.Projects.Queries.ProjectExistsById;
using Lucy.Application.Projects.Queries.ProjectExistsByKey;
using Lucy.Application.Validation;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.New;

/// <summary>
/// Validator for the <see cref="NewTagCommand"/> command.
/// </summary>
internal class NewTagCommandValidator(
    IMediator mediator) : ICommandValidator<NewTagCommand>
{
    /// <summary>
    /// The mediator instance for sending queries.
    /// </summary>
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ValidationResult> ValidateAsync(
        CommandContext context,
        NewTagCommand command,
        CancellationToken token = default)
    {
        var result = new ValidationResult();

        if (command.ProjectKey is null && command.ProjectId is null)
        {
            result.AddError(
                ConsoleValidationCode.ProjectKeyOrIdRequired,
                nameof(command.ProjectKey));
            return result;
        }

        // Validate that the project exists (unless --project-id is used)
        if (command.ProjectId is null)
        {
            var query = new ProjectExistsByKeyQuery(command.ProjectKey!);
            var exists = await _mediator.Send(query, token);

            if (!exists)
            {
                result.AddError(
                    ConsoleValidationCode.ProjectKeyNotFound,
                    nameof(command.ProjectKey),
                    command.ProjectKey!);
            }
        }

        return result;
    }
}
