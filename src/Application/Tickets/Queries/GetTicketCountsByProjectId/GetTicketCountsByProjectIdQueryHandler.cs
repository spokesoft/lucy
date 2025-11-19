using Lucy.Application.Interfaces;
using Lucy.Application.Tickets.DTOs;

namespace Lucy.Application.Tickets.Queries.GetTicketCountsByProjectId;

/// <summary>
/// Handler for <see cref="GetTicketCountsByProjectIdQuery"/>.
/// </summary>
public class GetTicketCountsByProjectIdQueryHandler(IReadOnlyUnitOfWork unitOfWork)
    : IRequestHandler<GetTicketCountsByProjectIdQuery, IEnumerable<TicketCountByStatusDto>>
{
    private readonly IReadOnlyUnitOfWork _unitOfWork = unitOfWork;

    /// <inheritdoc />
    public async Task<IEnumerable<TicketCountByStatusDto>> HandleAsync(
        GetTicketCountsByProjectIdQuery request,
        CancellationToken token = default)
        => await _unitOfWork.Tickets.GetTicketCountsByProjectIdAsync(request.ProjectId, token);
}
