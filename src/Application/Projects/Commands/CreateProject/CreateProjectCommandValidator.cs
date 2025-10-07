using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Validators;
using Lucy.Application.Validation;

namespace Lucy.Application.Projects.Commands.CreateProject;

/// <summary>
/// Validator for the CreateProjectCommand.
/// </summary>
public class CreateProjectCommandValidator(
    IReadOnlyUnitOfWork unitOfWork) : IRequestValidator<CreateProjectCommand>
{
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
    /// Asynchronously validates the given instance of CreateProjectCommand.
    /// </summary>
    public async Task<ValidationResult> ValidateAsync(CreateProjectCommand request, CancellationToken token = default)
    {
        var result = new ValidationResult();

        result.AddResult(await _keyValidator.ValidateAsync(request.Key, token));
        result.AddResult(_nameValidator.Validate(request.Name));
        result.AddResult(_descriptionValidator.Validate(request.Description));

        return result;
    }
}
