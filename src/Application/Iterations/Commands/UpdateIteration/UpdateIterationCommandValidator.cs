using Lucy.Application.Interfaces;
using Lucy.Application.Iterations.Validators;
using Lucy.Application.Validation;

namespace Lucy.Application.Iterations.Commands.UpdateIteration;

/// <summary>
/// Validator for the update iteration command.
/// </summary>
public class UpdateIterationCommandValidator(
    IReadOnlyUnitOfWork unitOfWork) : IRequestValidator<UpdateIterationCommand>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
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
    /// Asynchronously validates the given instance of update iteration command.
    /// </summary>
    public async Task<ValidationResult> ValidateAsync(UpdateIterationCommand request, CancellationToken token = default)
    {
        var result = new ValidationResult();

        var iteration = await _uow.Iterations.GetByIdAsync(request.Id, token);
        if (iteration is null)
        {
            return ValidationResult.Error(ValidationCode.IterationNotFound, nameof(request.Id), request.Id);
        }

        if (request.Name is null && request.Description is null && request.StartDate is null && request.EndDate is null)
            return ValidationResult.Error(ValidationCode.IterationNoDataToUpdate);

        if (request.Name is not null)
            result.AddResult(_nameValidator.Validate(request.Name));

        if (request.Description is not null)
            result.AddResult(_descriptionValidator.Validate(request.Description));

        var startDate = request.StartDate ?? iteration.StartDate;
        var endDate = request.EndDate ?? iteration.EndDate;
        result.AddResult(_dateValidator.Validate((StartDate: startDate, EndDate: endDate)));

        return result;
    }
}
