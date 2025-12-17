using Lucy.Application.Common.Interfaces;
using Lucy.Infrastructure.Logging.Services;
using Moq;

namespace Lucy.Console.Tests.Helpers;

/// <summary>
/// Test database logging service for unit testing purposes.
/// </summary>
public class TestDatabaseLoggingService : IDatabaseLoggingService
{
    public bool IsStarted { get; private set; }
    public bool IsStopped { get; private set; }
    public CancellationToken? StartToken { get; private set; }
    public Func<int, long, string>? StopMessageFactory { get; private set; }
    public TimeSpan StopDelay { get; set; } = TimeSpan.Zero;
    public Exception? StopException { get; set; }

    public void Start(CancellationToken? token = null)
    {
        IsStarted = true;
        StartToken = token;
    }

    public async Task StopAsync(Func<int, long, string>? finalMessageFactory = null)
    {
        StopMessageFactory = finalMessageFactory;

        if (StopDelay > TimeSpan.Zero)
        {
            await Task.Delay(StopDelay);
        }

        if (StopException != null)
        {
            throw StopException;
        }

        IsStopped = true;
    }

    public void Reset()
    {
        IsStarted = false;
        IsStopped = false;
        StartToken = null;
        StopMessageFactory = null;
        StopDelay = TimeSpan.Zero;
        StopException = null;
    }

    public void Dispose()
    {
        // Test implementation - no cleanup needed
    }
}
