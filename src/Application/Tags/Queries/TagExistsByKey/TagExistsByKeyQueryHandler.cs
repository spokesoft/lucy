using Lucy.Application.Interfaces;

namespace Lucy.Application.Tags.Queries.TagExistsByKey;

/// <summary>
/// Handler for checking if a tag exists by its key within a project.
/// </summary>
public class TagExistsByKeyQueryHandler(
    IReadOnlyUnitOfWork unitOfWork) : IRequestHandler<TagExistsByKeyQuery, bool>
{
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    public Task<bool> HandleAsync(TagExistsByKeyQuery request, CancellationToken token = default)
        => _uow.Tags.ExistsByKeyAsync(request.ProjectId, request.Key, token);
}
