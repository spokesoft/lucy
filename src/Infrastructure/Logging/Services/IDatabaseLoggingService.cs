namespace Lucy.Infrastructure.Logging.Services;

/// <summary>
/// Service interface for managing the lifecycle of the database logging service.
/// </summary>
public interface IDatabaseLoggingService : IDisposable
{
    /// <summary>
    /// Starts the background log processing task.
    /// </summary>
    void Start(CancellationToken? token = null);

    /// <summary>
    /// Stops the background log processing task gracefully.
    /// </summary>
    Task StopAsync(Func<int, long, string>? finalMessageFactory = null);
}
