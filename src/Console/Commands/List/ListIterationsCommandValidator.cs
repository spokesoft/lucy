using Lucy.Application.Common.Interfaces;
using Lucy.Application.Projects.Queries.ProjectExistsById;
using Lucy.Application.Projects.Queries.ProjectExistsByKey;
using Lucy.Application.Common.Validation;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.List;

/// <summary>
/// Validator for the <see cref="ListIterationsCommand"/> command.
/// </summary>
internal class ListIterationsCommandValidator(IMediator mediator) : ICommandValidator<ListIterationsCommand>
{
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ValidationResult> ValidateAsync(
        CommandContext context,
        ListIterationsCommand settings,
        CancellationToken token = default)
    {
        if (string.IsNullOrWhiteSpace(settings.ProjectKey) && settings.ProjectId is null)
        {
            return ValidationResult.Error(
                ConsoleValidationCode.ProjectKeyOrIdRequired,
                "Project key or ID is required.");
        }

        if (!string.IsNullOrWhiteSpace(settings.ProjectKey) && settings.ProjectId is not null)
        {
            return ValidationResult.Error(
                ConsoleValidationCode.ProjectKeyAndIdMutuallyExclusive,
                "Cannot specify both project key and ID.");
        }

        if (!string.IsNullOrWhiteSpace(settings.ProjectKey))
        {
            var query = new ProjectExistsByKeyQuery(settings.ProjectKey);
            if (!await _mediator.Send(query, token))
            {
                return ValidationResult.Error(
                    ConsoleValidationCode.ProjectKeyNotFound,
                    nameof(settings.ProjectKey),
                    settings.ProjectKey);
            }
        }

        if (settings.ProjectId is not null)
        {
            var query = new ProjectExistsByIdQuery(settings.ProjectId.Value);
            if (!await _mediator.Send(query, token))
            {
                return ValidationResult.Error(
                    ConsoleValidationCode.ProjectIdNotFound,
                    nameof(settings.ProjectId),
                    settings.ProjectId.Value.ToString());
            }
        }

        return ValidationResult.Success;
    }
}
