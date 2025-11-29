using Lucy.Application.Interfaces;
using Lucy.Domain.Entities;

namespace Lucy.Application.Tags.Commands.CreateTag;

/// <summary>
/// Handler for the CreateTagCommand.
/// </summary>
public class CreateTagCommandHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<CreateTagCommand, long>
{
    private readonly IUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Asynchronously handles the CreateTagCommand.
    /// </summary>
    public async Task<long> HandleAsync(CreateTagCommand request, CancellationToken token = default)
    {
        var tag = new Tag(
            request.ProjectId,
            request.Key,
            request.Label,
            request.Description);

        await _uow.Tags.AddAsync(tag, token);
        await _uow.SaveChangesAsync(token);
        return tag.Id;
    }
}
