using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Validators;
using Lucy.Application.Validation;

namespace Lucy.Application.Projects.Commands.UpdateProject;

/// <summary>
/// Validator for the update project command.
/// </summary>
public class UpdateProjectCommandValidator(
    IReadOnlyUnitOfWork unitOfWork) : IRequestValidator<UpdateProjectCommand>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Validator for project keys.
    /// </summary>
    private readonly ProjectKeyValidator _keyValidator = new(unitOfWork);

    /// <summary>
    /// Validator for project names.
    /// </summary>
    private readonly ProjectNameValidator _nameValidator = new();

    /// <summary>
    /// Validator for project descriptions.
    /// </summary>
    private readonly ProjectDescriptionValidator _descriptionValidator = new();

    /// <summary>
    /// Asynchronously validates the given instance of update project command.
    /// </summary>
    public async Task<ValidationResult> ValidateAsync(UpdateProjectCommand request, CancellationToken token = default)
    {
        var result = new ValidationResult();

        var project = await _uow.Projects.GetByIdAsync(request.Id, token);
        if (project is null)
        {
            return ValidationResult.Error(ValidationCode.ProjectNotFound, nameof(request.Id), request.Id);
        }

        if (request.Key is null && request.Name is null && request.Description is null)
            return ValidationResult.Error(ValidationCode.ProjectNoDataToUpdate);

        if (request.Key is not null)
        {
            var normalizedKey = request.Key.ToUpperInvariant();

            // Only validate key if it's being changed
            if (normalizedKey != project.Key)
                result.AddResult(await _keyValidator.ValidateAsync(request.Key, token));
            else
            {
                // Still validate format even if key isn't changing
                if (string.IsNullOrWhiteSpace(request.Key))
                    result.AddError(ValidationCode.ProjectKeyRequired, "Key");
                else if (!char.IsLetter(request.Key[0]))
                    result.AddError(ValidationCode.ProjectKeyStartWithLetter, "Key", request.Key);
                else if (request.Key.Length < 3 || request.Key.Length > 10)
                    result.AddError(ValidationCode.ProjectKeyLength, "Key", request.Key.Length);
                else if (!request.Key.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_'))
                    result.AddError(ValidationCode.ProjectKeyInvalidCharacters, "Key", request.Key);
            }
        }

        if (request.Name is not null)
            result.AddResult(_nameValidator.Validate(request.Name));

        if (request.Description is not null)
            result.AddResult(_descriptionValidator.Validate(request.Description));

        return result;
    }
}
