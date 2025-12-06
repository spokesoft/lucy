using Lucy.Application.Interfaces;
using Lucy.Domain.Entities;

namespace Lucy.Application.Tags.Commands.UpdateTag;

/// <summary>
/// Handler for the UpdateTagCommand.
/// </summary>
public class UpdateTagCommandHandler(
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateTagCommand>
{
    private readonly IUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Asynchronously handles the UpdateTagCommand.
    /// </summary>
    public async Task HandleAsync(UpdateTagCommand request, CancellationToken token = default)
    {
        var tag = await _uow.Tags.GetByIdAsync(request.Id, token)
            ?? throw new InvalidOperationException("Tag not found, cannot update.");


        if (request.Key is not null)
            tag.UpdateKey(request.Key);
        if (request.Label is not null)
            tag.UpdateLabel(request.Label);
        if (request.Description is not null)
            tag.UpdateDescription(request.Description);
        if (request.Color is not null)
            tag.UpdateColor(request.Color.Value);

        await _uow.SaveChangesAsync(token);
    }
}
