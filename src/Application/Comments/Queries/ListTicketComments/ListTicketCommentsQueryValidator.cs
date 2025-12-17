using Lucy.Application.Common.Interfaces;
using Lucy.Application.Common.Validation;

namespace Lucy.Application.Comments.Queries.ListTicketComments;

/// <summary>
/// Validator for the ListTicketCommentsQuery.
/// </summary>
public class ListTicketCommentsQueryValidator(
    IReadOnlyUnitOfWork unitOfWork) : IRequestValidator<ListTicketCommentsQuery>
{
    /// <summary>
    /// The unit of work for read-only operations.
    /// </summary>
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Asynchronously validates the ListTicketCommentsQuery.
    /// </summary>
    public async Task<ValidationResult> ValidateAsync(ListTicketCommentsQuery request, CancellationToken token = default)
    {
        if (!await _uow.Tickets.ExistsByIdAsync(request.TicketId, token))
            return ValidationResult.Error(ValidationCode.TicketNotFound, "TicketId", request.TicketId);

        return ValidationResult.Success;
    }
}
