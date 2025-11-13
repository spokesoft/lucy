namespace Lucy.Application.Statuses.Exceptions;

public class StatusNotFoundException(long statusId)
    : ApplicationException($"Status with ID '{statusId}' was not found.")
{
    public long StatusId { get; } = statusId;
}
