using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Queries.ProjectExistsByKey;
using Lucy.Application.Validation;

namespace Lucy.Application.Tags.Queries.TagExistsByKey;

/// <summary>
/// Validator for TagExistsByKeyQuery to ensure the project exists when checking by key.
/// </summary>
public class TagExistsByKeyQueryValidator(
    IReadOnlyUnitOfWork unitOfWork) : IRequestValidator<TagExistsByKeyQuery>
{
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    public async Task<ValidationResult> ValidateAsync(TagExistsByKeyQuery request, CancellationToken token = default)
    {
        var result = new ValidationResult();

        if (!await _uow.Projects.ExistsByIdAsync(request.ProjectId, token))
        {
            result.AddError(ValidationCode.ProjectNotFound, nameof(request.ProjectId), request.ProjectId);
        }

        return result;
    }
}
