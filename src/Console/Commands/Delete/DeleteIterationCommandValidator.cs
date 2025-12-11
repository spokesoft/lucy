using Lucy.Application.Interfaces;
using Lucy.Application.Iterations.Queries.IterationExistsByKey;
using Lucy.Application.Validation;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Delete;

/// <summary>
/// Validator for <see cref="DeleteIterationCommand"/>
/// </summary>
public class DeleteIterationCommandValidator(
    IMediator mediator) : ICommandValidator<DeleteIterationCommand>
{
    /// <summary>
    /// The mediator instance.
    /// </summary>
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ValidationResult> ValidateAsync(
        CommandContext context,
        DeleteIterationCommand command,
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

        return ValidationResult.Success;
    }
}
