using Lucy.Application.Common.Interfaces;
using Lucy.Application.Tags.DTOs;

namespace Lucy.Application.TicketTags.Queries.ListTagsByTicketId;

/// <summary>
/// Handler to list tags for a ticket.
/// </summary>
public class ListTagsByTicketIdQueryHandler(
    IReadOnlyUnitOfWork unitOfWork) : IRequestHandler<ListTagsByTicketIdQuery, IEnumerable<TagDto>>
{
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    public async Task<IEnumerable<TagDto>> HandleAsync(ListTagsByTicketIdQuery request, CancellationToken token = default)
    {
        var tags = await _uow.TicketTags.GetTagsByTicketIdAsync(request.TicketId, token);

        return tags.Select(tag => new TagDto
        {
            Id = tag.Id,
            ProjectId = tag.ProjectId,
            Key = tag.Key,
            Label = tag.Label,
            Description = tag.Description,
            Color = tag.Color,
            CreatedAt = tag.CreatedAt,
            UpdatedAt = tag.UpdatedAt
        });
    }
}
