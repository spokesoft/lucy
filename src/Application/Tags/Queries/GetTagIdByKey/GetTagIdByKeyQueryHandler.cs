using Lucy.Application.Common.Interfaces;

namespace Lucy.Application.Tags.Queries.GetTagIdByKey;

/// <summary>
/// Handler for getting a tag ID by its project and key.
/// </summary>
public class GetTagIdByKeyQueryHandler(
    IReadOnlyUnitOfWork unitOfWork) : IRequestHandler<GetTagIdByKeyQuery, long?>
{
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    public Task<long?> HandleAsync(GetTagIdByKeyQuery request, CancellationToken token = default)
        => _uow.Tags.GetByKeyAsync(request.ProjectId, request.Key.ToUpperInvariant(), token)
            .ContinueWith(task => task.Result?.Id, token);
}
