using Lucy.Application.Interfaces;
using Lucy.Application.Validation;

namespace Lucy.Application.TicketTags.Commands.RemoveTicketTag;

/// <summary>
/// Validator for the RemoveTicketTagCommand.
/// </summary>
public class RemoveTicketTagCommandValidator(
    IReadOnlyUnitOfWork unitOfWork) : IRequestValidator<RemoveTicketTagCommand>
{
    /// <summary>
    /// The unit of work for read-only operations.
    /// </summary>
    private readonly IReadOnlyUnitOfWork _unitOfWork = unitOfWork;

    /// <summary>
    /// Asynchronously validates the given instance of RemoveTicketTagCommand.
    /// </summary>
    public async Task<ValidationResult> ValidateAsync(RemoveTicketTagCommand request, CancellationToken token = default)
    {
        var result = new ValidationResult();

        if (!await _unitOfWork.Tags.ExistsByIdAsync(request.TagId, token))
        {
            result.AddError(ValidationCode.TagNotFound, "TagId", request.TagId);
            return result;
        }

        if (await _unitOfWork.Tickets.ExistsByIdAsync(request.TicketId, token))
        {
            result.AddError(ValidationCode.TicketNotFound, "TicketId", request.TicketId);
            return result;
        }

        return result;
    }
}
