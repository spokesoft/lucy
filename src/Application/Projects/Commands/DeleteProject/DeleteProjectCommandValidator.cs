using Lucy.Application.Interfaces;
using Lucy.Application.Validation;

namespace Lucy.Application.Projects.Commands.DeleteProject;

/// <summary>
/// Validator for the DeleteProjectCommand.
/// </summary>
public class DeleteProjectCommandValidator(
    IReadOnlyUnitOfWork unitOfWork) : IRequestValidator<DeleteProjectCommand>
{
    /// <summary>
    /// Unit of Work for managing repositories and transactions.
    /// </summary>
    private readonly IReadOnlyUnitOfWork _uow = unitOfWork;

    /// <summary>
    /// Asynchronously validates the DeleteProjectCommand.
    /// </summary>
    public async Task<ValidationResult> ValidateAsync(DeleteProjectCommand request, CancellationToken token = default)
    {
        var result = new ValidationResult();

        if (!await _uow.Projects.ExistsByIdAsync(request.Id, token))
            result.AddError(new ValidationError(
                ValidationCode.ProjectNotFound,
                nameof(request.Id),
                [request.Id]));

        return result;
    }
}
