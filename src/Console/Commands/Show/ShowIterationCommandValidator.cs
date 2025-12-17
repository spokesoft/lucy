using Lucy.Application.Common.Interfaces;
using Lucy.Application.Iterations.Queries.IterationExistsById;
using Lucy.Application.Iterations.Queries.IterationExistsByKey;
using Lucy.Application.Common.Validation;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Show;

/// <summary>
/// Validator for <see cref="ShowIterationCommand"/>
/// </summary>
public class ShowIterationCommandValidator(
    IMediator mediator) : ICommandValidator<ShowIterationCommand>
{
    /// <summary>
    /// The mediator instance.
    /// </summary>
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ValidationResult> ValidateAsync(
        CommandContext context,
        ShowIterationCommand command,
        CancellationToken token = default)
    {
        if (command.Id is null)
        {
            if (string.IsNullOrWhiteSpace(command.Key))
                return ValidationResult.Error(ConsoleValidationCode.IterationKeyOrIdRequired);

            var query = new IterationExistsByKeyQuery(command.Key);
            if (!await _mediator.Send(query, token))
            {
                return ValidationResult.Error(
                    ConsoleValidationCode.IterationKeyNotFound,
                    nameof(command.Key),
                    command.Key);
            }
        }
        else
        {
            var query = new IterationExistsByIdQuery(command.Id.Value);
            if (!await _mediator.Send(query, token))
            {
                return ValidationResult.Error(
                    ConsoleValidationCode.IterationNotFound,
                    nameof(command.Id),
                    command.Id.Value.ToString());
            }
        }

        return ValidationResult.Success;
    }
}
