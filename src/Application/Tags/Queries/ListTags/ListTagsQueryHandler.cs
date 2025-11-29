using Lucy.Application.Interfaces;
using Lucy.Application.Tags.DTOs;

namespace Lucy.Application.Tags.Queries.ListTags;

/// <summary>
/// Handler for listing all tags.
/// </summary>
public class ListTagsQueryHandler(
    IReadOnlyUnitOfWork unitOfWork) : IRequestHandler<ListTagsQuery, List<TagDto>>
{
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    public Task<List<TagDto>> HandleAsync(ListTagsQuery request, CancellationToken token = default)
        => _uow.Tags.GetAllAsync(token)
            .ContinueWith(task => task.Result.Select(tag => new TagDto
            {
                Id = tag.Id,
                ProjectId = tag.ProjectId,
                Key = tag.Key,
                Label = tag.Label,
                Description = tag.Description,
                Color = tag.Color,
                CreatedAt = tag.CreatedAt,
                UpdatedAt = tag.UpdatedAt
            }).ToList(), token);
}
