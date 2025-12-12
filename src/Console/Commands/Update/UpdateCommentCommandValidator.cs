using Lucy.Application.Comments.Queries.GetCommentById;
using Lucy.Application.Interfaces;
using Lucy.Application.Validation;
using Lucy.Console.Enums;
using Lucy.Console.Interfaces;
using Spectre.Console.Cli;

namespace Lucy.Console.Commands.Update;

/// <summary>
/// Validator for the <see cref="UpdateCommentCommand"/> command.
/// </summary>
internal class UpdateCommentCommandValidator(
    IMediator mediator) : ICommandValidator<UpdateCommentCommand>
{
    private readonly IMediator _mediator = mediator;

    /// <inheritdoc />
    public async Task<ValidationResult> ValidateAsync(
        CommandContext context,
        UpdateCommentCommand command,
        CancellationToken token = default)
    {
        var result = new ValidationResult();

        // Check if comment exists
        var query = new GetCommentByIdQuery(command.Id);
        var comment = await _mediator.Send(query, token);

        if (comment is null)
        {
            result.AddError(
                ConsoleValidationCode.CommentNotFound,
                nameof(command.Id));
            return result;
        }

        // Validate content is provided
        if (string.IsNullOrWhiteSpace(command.Content))
        {
            result.AddError(
                ConsoleValidationCode.CommentContentRequired,
                nameof(command.Content));
            return result;
        }

        return result;
    }
}
