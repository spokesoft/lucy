using Lucy.Application.Interfaces;
using Lucy.Application.Validation;

namespace Lucy.Application.Tickets.Commands.UnassignTicketFromIteration;

/// <summary>
/// Validator for UnassignTicketFromIterationCommand.
/// </summary>
public class UnassignTicketFromIterationCommandValidator(IUnitOfWork unitOfWork) : IAsyncValidator<UnassignTicketFromIterationCommand>
{
    private readonly IUnitOfWork _uow = unitOfWork;

    public async Task<ValidationResult> ValidateAsync(UnassignTicketFromIterationCommand request, CancellationToken token = default)
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
