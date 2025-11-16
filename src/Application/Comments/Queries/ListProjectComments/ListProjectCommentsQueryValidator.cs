using Lucy.Application.Interfaces;
using Lucy.Application.Validation;

namespace Lucy.Application.Comments.Queries.ListProjectComments;

/// <summary>
/// Validator for the ListProjectCommentsQuery.
/// </summary>
public class ListProjectCommentsQueryValidator(
    IReadOnlyUnitOfWork unitOfWork) : IRequestValidator<ListProjectCommentsQuery>
{
    /// <summary>
    /// The unit of work for read-only operations.
    /// </summary>
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Asynchronously validates the ListProjectCommentsQuery.
    /// </summary>
    public async Task<ValidationResult> ValidateAsync(ListProjectCommentsQuery request, CancellationToken token = default)
    {
        if (!await _uow.Projects.ExistsByIdAsync(request.ProjectId, token))
            return ValidationResult.Error(ValidationCode.ProjectNotFound, "ProjectId", request.ProjectId);

        return ValidationResult.Success;
    }
}
