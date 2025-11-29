using Lucy.Application.Interfaces;
using Lucy.Domain.Entities;

namespace Lucy.Application.TicketTags.Commands.AddTicketTag;

/// <summary>
/// Handler for the AddTicketTagCommand.
/// </summary>
public class AddTicketTagCommandHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<AddTicketTagCommand>
{
    private readonly IUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Asynchronously handles the AddTicketTagCommand.
    /// </summary>
    public async Task HandleAsync(AddTicketTagCommand request, CancellationToken token = default)
    {
        var ticket = await _uow.Tickets.GetByIdAsync(request.TicketId, token);
        var tag = await _uow.Tags.GetByIdAsync(request.TagId, token);

        var TicketTag = new TicketTag(ticket!, tag!);
        await _uow.TicketTags.AddAsync(TicketTag, token);
        await _uow.SaveChangesAsync(token);
    }
}
