using Lucy.Application.Common.Interfaces;

namespace Lucy.Application.Tags.Queries.GetTagIdByKey;

/// <summary>
/// Query to get a tag ID by its project and key.
/// </summary>
/// <param name="ProjectId">The ID of the project.</param>
/// <param name="Key">The key of the tag.</param>
public record GetTagIdByKeyQuery(long ProjectId, string Key) : IRequest<long?>;
