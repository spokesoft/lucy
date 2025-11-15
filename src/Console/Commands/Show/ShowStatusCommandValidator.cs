using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectByKey;
using Lucy.Application.Validation;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Show;

/// <summary>
/// Validator for <see cref="ShowStatusCommand"/>
/// </summary>
internal class ShowStatusCommandValidator(
    IMediator mediator) : ICommandValidator<ShowStatusCommand>
{
    /// <summary>
    /// The mediator instance for sending queries.
    /// </summary>
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ValidationResult> ValidateAsync(
        CommandContext context,
        ShowStatusCommand command,
        CancellationToken token = default)
    {
        // If using status ID, no other validation needed
        if (command.Id is not null)
            return ValidationResult.Success;

        // If using status key, need project key or ID
        if (string.IsNullOrWhiteSpace(command.Key))
            return ValidationResult.Error(ConsoleValidationCode.StatusKeyOrIdRequired);

        if (command.ProjectKey is null && command.ProjectId is null)
        {
            return ValidationResult.Error(
                ConsoleValidationCode.ProjectKeyOrIdRequiredForStatusKey,
                nameof(command.ProjectKey));
        }

        // If ProjectKey is provided, validate that the project exists
        if (command.ProjectKey is not null)
        {
            var query = new GetProjectByKeyQuery(command.ProjectKey);
            var project = await _mediator.Send(query, token);

            if (project is null)
            {
                return ValidationResult.Error(
                    ConsoleValidationCode.ProjectKeyNotFound,
                    nameof(command.ProjectKey),
                    command.ProjectKey);
            }
        }

        return ValidationResult.Success;
    }
}
