using Lucy.Application.Common.Interfaces;

namespace Lucy.Application.Tags.Commands.DeleteTag;

/// <summary>
/// Command to delete a tag by its ID.
/// </summary>
/// <param name="Id">The unique identifier of the tag to be deleted.</param>
public record DeleteTagCommand(long Id) : IRequest;
