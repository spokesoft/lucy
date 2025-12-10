namespace Lucy.Application.Iterations.Exceptions;

public class IterationNotFoundException(long iterationId)
    : ApplicationException($"Iteration with ID '{iterationId}' was not found.")
{
    public long IterationId { get; } = iterationId;
}
