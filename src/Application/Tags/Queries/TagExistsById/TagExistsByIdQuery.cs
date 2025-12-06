using Lucy.Application.Interfaces;

namespace Lucy.Application.Tags.Queries.TagExistsById;

/// <summary>
/// Query to check if a tag exists by its ID.
/// </summary>
/// <param name="Id">The ID of the tag to check for.</param>
public record TagExistsByIdQuery(long Id) : IRequest<bool>;
