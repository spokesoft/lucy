using Lucy.Application.Common.Interfaces;
using Lucy.Application.Tickets.DTOs;

namespace Lucy.Application.Tickets.Queries.GetTicketCountsByIterationId;

/// <summary>
/// Handler for <see cref="GetTicketCountsByIterationIdQuery"/>.
/// </summary>
public class GetTicketCountsByIterationIdQueryHandler(IReadOnlyUnitOfWork unitOfWork)
    : IRequestHandler<GetTicketCountsByIterationIdQuery, IEnumerable<TicketCountByStatusDto>>
{
    private readonly IReadOnlyUnitOfWork _unitOfWork = unitOfWork;

    /// <inheritdoc />
    public async Task<IEnumerable<TicketCountByStatusDto>> HandleAsync(
        GetTicketCountsByIterationIdQuery request,
        CancellationToken token = default)
        => await _unitOfWork.Tickets.GetTicketCountsByIterationIdAsync(request.IterationId, token);
}
