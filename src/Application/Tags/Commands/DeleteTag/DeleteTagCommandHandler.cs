using Lucy.Application.Common.Interfaces;

namespace Lucy.Application.Tags.Commands.DeleteTag;

/// <summary>
/// Handler for the DeleteTagCommand.
/// </summary>
public class DeleteTagCommandHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteTagCommand>
{
    private readonly IUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Asynchronously handles the DeleteTagCommand.
    /// </summary>
    public async Task HandleAsync(DeleteTagCommand request, CancellationToken token = default)
    {
        var tag = await _uow.Tags.GetByIdAsync(request.Id, token)
            ?? throw new InvalidOperationException("Tag not found, cannot delete.");

        _uow.Tags.Remove(tag);
        await _uow.SaveChangesAsync(token);
    }
}
