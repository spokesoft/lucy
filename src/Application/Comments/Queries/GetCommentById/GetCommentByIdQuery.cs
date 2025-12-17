using Lucy.Application.Comments.DTOs;
using Lucy.Application.Common.Interfaces;

namespace Lucy.Application.Comments.Queries.GetCommentById;

/// <summary>
/// Query to get a comment by its ID.
/// </summary>
public record GetCommentByIdQuery(long Id) : IRequest<CommentDto?>;
