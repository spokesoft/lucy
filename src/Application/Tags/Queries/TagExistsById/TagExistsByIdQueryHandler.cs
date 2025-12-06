using Lucy.Application.Interfaces;

namespace Lucy.Application.Tags.Queries.TagExistsById;

/// <summary>
/// Handler for checking if a tag exists by its ID.
/// </summary>
public class TagExistsByIdQueryHandler(
    IReadOnlyUnitOfWork unitOfWork) : IRequestHandler<TagExistsByIdQuery, bool>
{
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    public Task<bool> HandleAsync(TagExistsByIdQuery request, CancellationToken token = default)
        => _uow.Tags.ExistsByIdAsync(request.Id, token);
}
