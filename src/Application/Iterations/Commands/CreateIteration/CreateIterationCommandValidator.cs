using Lucy.Application.Interfaces;
using Lucy.Application.Iterations.Validators;
using Lucy.Application.Validation;

namespace Lucy.Application.Iterations.Commands.CreateIteration;

/// <summary>
/// Validator for the CreateIterationCommand.
/// </summary>
public class CreateIterationCommandValidator(
    IReadOnlyUnitOfWork unitOfWork) : IRequestValidator<CreateIterationCommand>
{
    /// <summary>
    /// Read-only unit of work for querying repositories.
    /// </summary>
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Validator for iteration names.
    /// </summary>
    private readonly IterationNameValidator _nameValidator = new();

    /// <summary>
    /// Validator for iteration descriptions.
    /// </summary>
    private readonly IterationDescriptionValidator _descriptionValidator = new();

    /// <summary>
    /// Validator for iteration date ranges.
    /// </summary>
    private readonly IterationDateValidator _dateValidator = new();

    /// <summary>
    /// Asynchronously validates the given instance of CreateIterationCommand.
    /// </summary>
    public async Task<ValidationResult> ValidateAsync(CreateIterationCommand request, CancellationToken token = default)
    {
        var result = new ValidationResult();

        if (!await _uow.Projects.ExistsByIdAsync(request.ProjectId, token))
            result.AddError(ValidationCode.ProjectNotFound, "ProjectId", request.ProjectId);

        result.AddResult(_nameValidator.Validate(request.Name));
        result.AddResult(_descriptionValidator.Validate(request.Description));
        result.AddResult(_dateValidator.Validate((request.StartDate, request.EndDate)));

        return result;
    }
}
