using Lucy.Application.Comments.Queries.GetCommentById;
using Lucy.Application.Interfaces;
using Lucy.Application.Validation;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Delete;

/// <summary>
/// Validator for the <see cref="DeleteCommentCommand"/> command.
/// </summary>
internal class DeleteCommentCommandValidator(
    IMediator mediator) : ICommandValidator<DeleteCommentCommand>
{
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ValidationResult> ValidateAsync(
        CommandContext context,
        DeleteCommentCommand command,
        CancellationToken token = default)
    {
        var result = new ValidationResult();

        // Check if comment exists
        var query = new GetCommentByIdQuery(command.Id);
        var comment = await _mediator.Send(query, token);

        if (comment is null)
        {
            result.AddError(ValidationResult.Error(
                ConsoleValidationCode.CommentNotFound,
                nameof(command.Id)).Errors.First());
            return result;
        }

        return result;
    }
}
