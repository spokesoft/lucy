using Lucy.Application.Interfaces;
using Lucy.Application.Tickets.Validators;
using Lucy.Application.Validation;

namespace Lucy.Application.Tickets.Commands.CreateTicket;

/// <summary>
/// Validator for the CreateTicketCommand.
/// </summary>
public class CreateTicketCommandValidator(
    IReadOnlyUnitOfWork unitOfWork) : IRequestValidator<CreateTicketCommand>
{
    /// <summary>
    /// The unit of work for read-only operations.
    /// </summary>
    private readonly IReadOnlyUnitOfWork _unitOfWork = unitOfWork;

    /// <summary>
    /// Validator for ticket titles.
    /// </summary>
    private readonly TicketTitleValidator _titleValidator = new();

    /// <summary>
    /// Validator for ticket descriptions.
    /// </summary>
    private readonly TicketDescriptionValidator _descriptionValidator = new();

    /// <summary>
    /// Asynchronously validates the given instance of CreateTicketCommand.
    /// </summary>
    public async Task<ValidationResult> ValidateAsync(CreateTicketCommand request, CancellationToken token = default)
    {
        var result = new ValidationResult();

        if (!await _unitOfWork.Projects.ExistsByIdAsync(request.ProjectId, token))
        {
            result.AddError(ValidationCode.ProjectNotFound, "ProjectId", request.ProjectId);
            return result;
        }

        if (!await _unitOfWork.Statuses.ExistsByIdAsync(request.StatusId, token))
        {
            result.AddError(ValidationCode.StatusNotFound, "StatusId", request.StatusId);
            return result;
        }

        // Verify the status belongs to the project
        var status = await _unitOfWork.Statuses.GetByIdAsync(request.StatusId, token);
        if (status?.ProjectId != request.ProjectId)
        {
            result.AddError(ValidationCode.StatusNotInProject, "StatusId", request.StatusId);
            return result;
        }

        result.AddResult(_titleValidator.Validate(request.Title));
        result.AddResult(_descriptionValidator.Validate(request.Description));

        return result;
    }
}
