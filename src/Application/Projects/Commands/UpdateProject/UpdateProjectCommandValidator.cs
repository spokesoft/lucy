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

        if (!await _uow.Projects.ExistsByIdAsync(request.Id, token))
        {
            result.AddError(new ValidationError(
                ValidationCode.ProjectNotFound,
                nameof(request.Id),
                [request.Id]));
        }
        else
        {
            if (request.Key is null && request.Name is null && request.Description is null)
                result.AddError(new ValidationError(
                    ValidationCode.ProjectNoDataToUpdate));

            if (request.Key is not null)
                result.AddResult(await _keyValidator.ValidateAsync(request.Key, token));

            if (request.Name is not null)
                result.AddResult(_nameValidator.Validate(request.Name));

            if (request.Description is not null)
                result.AddResult(_descriptionValidator.Validate(request.Description));
        }

        return result;
    }
}
