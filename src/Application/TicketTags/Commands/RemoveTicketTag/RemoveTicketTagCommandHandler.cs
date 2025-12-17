using Lucy.Application.Common.Interfaces;

namespace Lucy.Application.TicketTags.Commands.RemoveTicketTag;

/// <summary>
/// Handler for the RemoveTicketTagCommand.
/// </summary>
public class RemoveTicketTagCommandHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<RemoveTicketTagCommand>
{
    private readonly IUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Asynchronously handles the RemoveTicketTagCommand.
    /// </summary>
    public async Task HandleAsync(RemoveTicketTagCommand request, CancellationToken token = default)
    {
        var ticketTag = await _uow.TicketTags.GetByTicketAndTagAsync(request.TicketId, request.TagId, token);

        if (ticketTag is null)
            throw new InvalidOperationException("Tag not found on ticket, cannot remove.");

        _uow.TicketTags.Remove(ticketTag);
        await _uow.SaveChangesAsync(token);
    }
}
