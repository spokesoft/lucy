using Lucy.Application.Common.Interfaces;
using Lucy.Application.Common.Validation;

namespace Lucy.Application.Tickets.Commands.DeleteTicket;

/// <summary>
/// Validator for the DeleteTicketCommand.
/// </summary>
public class DeleteTicketCommandValidator(
    IReadOnlyUnitOfWork unitOfWork) : IRequestValidator<DeleteTicketCommand>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Asynchronously validates the DeleteTicketCommand.
    /// </summary>
    public async Task<ValidationResult> ValidateAsync(DeleteTicketCommand request, CancellationToken token = default)
    {
        if (!await _uow.Tickets.ExistsByIdAsync(request.Id, token))
            return ValidationResult.Error(ValidationCode.TicketNotFound, "Id", request.Id);

        return ValidationResult.Success;
    }
}
