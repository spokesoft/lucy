using Lucy.Application.Interfaces;
using Lucy.Application.Tags.DTOs;

namespace Lucy.Application.Tags.Queries.GetTagById;

/// <summary>
/// Handler for getting a tag by its ID.
/// </summary>
public class GetTagByIdQueryHandler(
    IReadOnlyUnitOfWork unitOfWork) : IRequestHandler<GetTagByIdQuery, TagDto?>
{
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    public Task<TagDto?> HandleAsync(GetTagByIdQuery request, CancellationToken token = default)
        => _uow.Tags.GetByIdAsync(request.Id, token)
            .ContinueWith(task => task.Result is not null ? new TagDto
            {
                Id = task.Result.Id,
                ProjectId = task.Result.ProjectId,
                Key = task.Result.Key,
                Label = task.Result.Label,
                Description = task.Result.Description,
                Color = task.Result.Color,
                CreatedAt = task.Result.CreatedAt,
                UpdatedAt = task.Result.UpdatedAt
            } : null, token);
}
