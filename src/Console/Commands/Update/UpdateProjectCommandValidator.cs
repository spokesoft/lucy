using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Queries.ProjectExistsByKey;
using Lucy.Application.Validation;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Update;

/// <summary>
/// Validator for <see cref="UpdateProjectCommand"/>
/// </summary>
public class UpdateProjectCommandValidator(
    IMediator mediator) : ICommandValidator<UpdateProjectCommand>
{
    /// <summary>
    /// The mediator instance.
    /// </summary>
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ValidationResult> ValidateAsync(
        CommandContext context,
        UpdateProjectCommand command,
        CancellationToken token = default)
    {
        if (command.Id is null)
        {
            if (string.IsNullOrWhiteSpace(command.Key))
                return ValidationResult.Error(ConsoleValidationCode.ProjectKeyOrIdRequired);

            var query = new ProjectExistsByKeyQuery(command.Key);
            if (!await _mediator.Send(query, token))
            {
                return ValidationResult.Error(
                    ConsoleValidationCode.ProjectKeyNotFound,
                    nameof(command.Key),
                    command.Key);
            }
        }

        return ValidationResult.Success;
    }
}
