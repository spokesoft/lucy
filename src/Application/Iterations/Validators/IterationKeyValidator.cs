using Lucy.Application.Common.Interfaces;
using Lucy.Application.Common.Validation;

namespace Lucy.Application.Iterations.Validators;

/// <summary>
/// Validator for iteration keys.
/// </summary>
public class IterationKeyValidator(IReadOnlyUnitOfWork unitOfWork) : IAsyncValidator<string>
{
    /// <summary>
    /// Read-only unit of work for querying repositories.
    /// </summary>
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Asynchronously validates the given iteration key.
    /// </summary>
    public async Task<ValidationResult> ValidateAsync(string key, CancellationToken token = default)
    {
        var result = new ValidationResult();

        if (string.IsNullOrWhiteSpace(key))
            return ValidationResult.Error(ValidationCode.IterationKeyRequired, "Key");

        if (key.Length > 50)
            result.AddError(ValidationCode.IterationKeyLength, "Key", key.Length);

        if (result.IsValid && await _uow.Iterations.ExistsByKeyAsync(key, token))
            result.AddError(ValidationCode.IterationKeyExists, "Key", key);

        return result;
    }
}
