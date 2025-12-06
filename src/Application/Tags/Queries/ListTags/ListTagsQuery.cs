using Lucy.Application.Interfaces;
using Lucy.Application.Tags.DTOs;

namespace Lucy.Application.Tags.Queries.ListTags;

/// <summary>
/// Query to list all tags for a specific project.
/// </summary>
/// <param name="ProjectId">The ID of the project.</param>
public record ListTagsQuery(
    long ProjectId) : IRequest<List<TagDto>>;
