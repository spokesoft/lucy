namespace Lucy.Application.Projects.Exceptions;

public class ProjectNotFoundException(long projectId)
    : ApplicationException($"Project with ID '{projectId}' was not found.")
{
    public long ProjectId { get; } = projectId;
}
