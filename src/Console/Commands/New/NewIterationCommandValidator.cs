using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectByKey;
using Lucy.Application.Validation;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.New;

/// <summary>
/// Validator for the <see cref="NewIterationCommand"/> command.
/// </summary>
internal class NewIterationCommandValidator(
    IMediator mediator) : ICommandValidator<NewIterationCommand>
{
    /// <summary>
    /// The mediator instance for sending queries.
    /// </summary>
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ValidationResult> ValidateAsync(
        CommandContext context,
        NewIterationCommand command,
        CancellationToken token = default)
    {
        var result = new ValidationResult();

        // Validate that either ProjectKey or ProjectId is provided
        if (command.ProjectKey is null && command.ProjectId is null)
        {
            result.AddError(
                ConsoleValidationCode.ProjectKeyOrIdRequired,
                nameof(command.ProjectKey));
            return result;
        }

        // Resolve and validate ProjectId
        if (command.ProjectKey is not null)
        {
            var projectQuery = new GetProjectByKeyQuery(command.ProjectKey);
            var project = await _mediator.Send(projectQuery, token);

            if (project is null)
            {
                result.AddError(
                    ConsoleValidationCode.ProjectKeyNotFound,
                    nameof(command.ProjectKey),
                    command.ProjectKey);
                return result;
            }
        }

        // Validate date range if both dates are provided
        if (command.StartDate.HasValue && command.EndDate.HasValue && command.StartDate > command.EndDate)
        {
            result.AddError(
                ConsoleValidationCode.InvalidDateRange,
                nameof(command.StartDate));
        }

        return result;
    }
}
