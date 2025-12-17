using Lucy.Application.Common.Interfaces;
using Lucy.Application.Common.Validation;

namespace Lucy.Application.TicketTags.Commands.AddTicketTag;

/// <summary>
/// Validator for the AddTicketTagCommand.
/// </summary>
public class AddTicketTagCommandValidator(
    IReadOnlyUnitOfWork unitOfWork) : IRequestValidator<AddTicketTagCommand>
{
    /// <summary>
    /// The unit of work for read-only operations.
    /// </summary>
    private readonly IReadOnlyUnitOfWork _unitOfWork = unitOfWork;

    /// <summary>
    /// Asynchronously validates the given instance of AddTicketTagCommand.
    /// </summary>
    public async Task<ValidationResult> ValidateAsync(AddTicketTagCommand request, CancellationToken token = default)
    {
        var result = new ValidationResult();

        if (!await _unitOfWork.Tags.ExistsByIdAsync(request.TagId, token))
        {
            result.AddError(ValidationCode.TagNotFound, "TagId", request.TagId);
            return result;
        }

        if (!await _unitOfWork.Tickets.ExistsByIdAsync(request.TicketId, token))
        {
            result.AddError(ValidationCode.TicketNotFound, "TicketId", request.TicketId);
            return result;
        }

        return result;
    }
}
