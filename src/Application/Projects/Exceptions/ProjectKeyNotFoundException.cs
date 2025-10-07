namespace Lucy.Application.Projects.Exceptions;

public class ProjectKeyNotFoundException(string projectKey)
    : ApplicationException($"Project with key '{projectKey}' was not found.")
{
    public string ProjectKey { get; } = projectKey;
}
