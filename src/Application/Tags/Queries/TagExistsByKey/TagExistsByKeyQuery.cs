using Lucy.Application.Common.Interfaces;

namespace Lucy.Application.Tags.Queries.TagExistsByKey;

/// <summary>
/// Query to check if a tag exists by its key within a project.
/// </summary>
/// <param name="ProjectId">The ID of the project.</param>
/// <param name="Key">The key of the tag.</param>
public record TagExistsByKeyQuery(long ProjectId, string Key) : IRequest<bool>;
