using Lucy.Application.Interfaces;
using Lucy.Application.Tags.DTOs;

namespace Lucy.Application.Tags.Queries.ListTags;

/// <summary>
/// Query to list all tags.
/// </summary>
public record ListTagsQuery() : IRequest<List<TagDto>>;
