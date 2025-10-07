using Lucy.Application.Interfaces;
using Lucy.Application.Validation;

namespace Lucy.Application.Projects.Validators;

/// <summary>
/// Validator for project keys.
/// </summary>
public class ProjectKeyValidator(IReadOnlyUnitOfWork unitOfWork) : IAsyncValidator<string>
{
    /// <summary>
    /// Read-only unit of work for querying repositories.
    /// </summary>
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Asynchronously validates the given project key.
    /// </summary>
    public async Task<ValidationResult> ValidateAsync(string key, CancellationToken token = default)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(key))
            result.AddError(new ValidationError(
                ValidationCode.ProjectKeyRequired,
                nameof(key)));

        if (result.IsValid && (key.Length < 3 || key.Length > 10))
            result.AddError(new ValidationError(
                ValidationCode.ProjectKeyLength,
                nameof(key),
                [key.Length]));

        if (result.IsValid && !key.All(char.IsLetterOrDigit))
            result.AddError(new ValidationError(
                ValidationCode.ProjectKeyAlphaNumeric,
                nameof(key),
                [key]));

        if (result.IsValid && await _uow.Projects.ExistsByKeyAsync(key, token))
            result.AddError(new ValidationError(
                ValidationCode.ProjectKeyExists,
                nameof(key),
                [key]));

        return result;
    }
}
