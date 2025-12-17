using Lucy.Application.Common.Interfaces;
using Lucy.Application.Tickets.Validators;
using Lucy.Application.Common.Validation;

namespace Lucy.Application.Tickets.Commands.UpdateTicket;

/// <summary>
/// Validator for the UpdateTicketCommand.
/// </summary>
public class UpdateTicketCommandValidator(
    IReadOnlyUnitOfWork unitOfWork) : IRequestValidator<UpdateTicketCommand>
{
    /// <summary>
    /// Read-only unit of work for querying repositories.
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
    /// Asynchronously validates the given instance of UpdateTicketCommand.
    /// </summary>
    public async Task<ValidationResult> ValidateAsync(UpdateTicketCommand request, CancellationToken token = default)
    {
        var result = new ValidationResult();

        var ticket = await _unitOfWork.Tickets.GetByIdAsync(request.Id, token);
        if (ticket == null)
        {
            result.AddError(ValidationCode.TicketNotFound, "Id", request.Id);
            return result;
        }

        // Check if there is any data to update
        if (!request.StatusId.HasValue && request.Title == null && request.Description == null)
        {
            result.AddError(ValidationCode.TicketNoDataToUpdate, "Command");
            return result;
        }

        if (request.StatusId.HasValue)
        {
            if (!await _unitOfWork.Statuses.ExistsByIdAsync(request.StatusId.Value, token))
            {
                result.AddError(ValidationCode.StatusNotFound, "StatusId", request.StatusId.Value);
                return result;
            }

            // Verify the status belongs to the same project
            var status = await _unitOfWork.Statuses.GetByIdAsync(request.StatusId.Value, token);
            if (status?.ProjectId != ticket.ProjectId)
            {
                result.AddError(ValidationCode.StatusNotInProject, "StatusId", request.StatusId.Value);
                return result;
            }
        }

        if (request.Title != null)
            result.AddResult(_titleValidator.Validate(request.Title));

        if (request.Description != null)
            result.AddResult(_descriptionValidator.Validate(request.Description));

        return result;
    }
}
