using Lucy.Application.Interfaces;
using Lucy.Application.Validation;

namespace Lucy.Application.Comments.Validators;

/// <summary>
/// Validator for comment content.
/// </summary>
public class CommentContentValidator : IValidator<string>
{
    /// <summary>
    /// Validates the given comment content.
    /// </summary>
    public ValidationResult Validate(string content)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(content))
        {
            result.AddError(ValidationCode.CommentContentRequired, "Content", "Content is required.");
        }
        else if (content.Length > 5000)
        {
            result.AddError(ValidationCode.CommentContentLength, "Content", "Content must not exceed 5000 characters.");
        }

        return result;
    }
}
