using Lucy.Application.Interfaces;

namespace Lucy.Application.Projects.Queries.GetProjectIdByKey;

/// <summary>
/// Handler for getting a project ID by its key.
/// </summary>
public class GetProjectIdByKeyQueryHandler(
    IReadOnlyUnitOfWork unitOfWork) : IRequestHandler<GetProjectIdByKeyQuery, long?>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Handles the query to get a project ID by its key.
    /// </summary>
    public Task<long?> HandleAsync(GetProjectIdByKeyQuery request, CancellationToken token = default)
        => _uow.Projects.GetByKeyAsync(request.Key, token)
            .ContinueWith(task => task.Result?.Id, token);
}
