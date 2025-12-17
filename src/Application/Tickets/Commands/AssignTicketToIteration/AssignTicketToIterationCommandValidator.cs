using Lucy.Application.Common.Interfaces;
using Lucy.Application.Common.Validation;

namespace Lucy.Application.Tickets.Commands.AssignTicketToIteration;

/// <summary>
/// Validator for AssignTicketToIterationCommand.
/// </summary>
public class AssignTicketToIterationCommandValidator(IUnitOfWork unitOfWork) : IAsyncValidator<AssignTicketToIterationCommand>
{
    private readonly IUnitOfWork _uow = unitOfWork;

    public async Task<ValidationResult> ValidateAsync(AssignTicketToIterationCommand request, CancellationToken token = default)
    {
        var result = new ValidationResult();

        if (await _uow.Tickets.GetByIdAsync(request.TicketId, token) is null)
        {
            result.AddError(nameof(request.TicketId), "Ticket not found.");
        }

        if (await _uow.Iterations.GetByIdAsync(request.IterationId, token) is null)
        {
            result.AddError(nameof(request.IterationId), "Iteration not found.");
        }

        return result;
    }
}
