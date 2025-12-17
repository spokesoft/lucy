using Lucy.Application.Common.Interfaces;
using Lucy.Application.Tags.DTOs;

namespace Lucy.Application.Tags.Queries.GetTagById;

/// <summary>
/// Query to get a tag by its ID.
/// </summary>
/// <param name="Id">The ID of the tag to retrieve.</param>
public record GetTagByIdQuery(long Id) : IRequest<TagDto?>;
