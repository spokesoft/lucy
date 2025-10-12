using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Queries.ProjectExistsByKey;
using Lucy.Application.Validation;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Show;

/// <summary>
/// Validator for <see cref="ShowProjectCommand"/>
/// </summary>
public class ShowProjectCommandValidator(
    IMediator mediator) : ICommandValidator<ShowProjectCommand>
{
    /// <summary>
    /// The mediator instance.
    /// </summary>
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ValidationResult> ValidateAsync(
        CommandContext context,
        ShowProjectCommand command,
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
