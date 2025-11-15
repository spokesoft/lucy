using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Queries.GetProjectByKey;
using Lucy.Application.Validation;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.New;

/// <summary>
/// Validator for the <see cref="NewStatusCommand"/> command.
/// </summary>
internal class NewStatusCommandValidator(
    IMediator mediator) : ICommandValidator<NewStatusCommand>
{
    /// <summary>
    /// The mediator instance for sending queries.
    /// </summary>
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ValidationResult> ValidateAsync(
        CommandContext context,
        NewStatusCommand command,
        CancellationToken token = default)
    {
        var result = new ValidationResult();

        // Validate that either ProjectKey or ProjectId is provided
        if (command.ProjectKey is null && command.ProjectId is null)
        {
            result.AddError(new ValidationError(
                "Either ProjectKey or ProjectId must be provided.",
                nameof(command.ProjectKey)));
            return result;
        }

        // If ProjectKey is provided, validate that the project exists
        if (command.ProjectKey is not null)
        {
            var query = new GetProjectByKeyQuery(command.ProjectKey);
            var project = await _mediator.Send(query, token);

            if (project is null)
            {
                result.AddError(new ValidationError(
                    $"Project with key '{command.ProjectKey}' not found.",
                    nameof(command.ProjectKey)));
            }
        }

        return result;
    }
}
