using System.Threading.Channels;
using Lucy.Infrastructure.Logging.Database;
using Lucy.Infrastructure.Logging.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Lucy.Infrastructure.Logging.Services;

/// <summary>
/// Service that processes log entries from a channel and writes them to the database.
/// </summary>
public class DatabaseLoggingService(
    IServiceProvider provider,
    Channel<LogEntry> channel,
    IOptions<DatabaseLoggingOptions> options) : IDatabaseLoggingService
{
    /// <summary>
    /// The service provider to create scopes for database contexts.
    /// </summary>
    private readonly IServiceProvider _provider = provider;

    /// <summary>
    /// The channel writer to enqueue log entries.
    /// </summary>
    private readonly ChannelWriter<LogEntry> _writer = channel.Writer;

    /// <summary>
    /// The channel reader to dequeue log entries.
    /// </summary>
    private readonly ChannelReader<LogEntry> _reader = channel.Reader;

    /// <summary>
    /// The logging options.
    /// </summary>
    private readonly DatabaseLoggingOptions _options = options.Value;

    /// <summary>
    /// Cancellation token source to signal stopping the background task.
    /// </summary>
    private CancellationTokenSource? _cts;

    /// <summary>
    /// The background task processing log entries.
    /// </summary>
    private Task? _processingTask;

    /// <summary>
    /// Starts the background log processing task.
    /// </summary>
    public void Start(CancellationToken? token = null)
    {
        _cts = token is not null
            ? CancellationTokenSource.CreateLinkedTokenSource(token.Value)
            : new CancellationTokenSource();
        _processingTask = Task.Run(ProcessLogQueueAsync);
    }

    /// <summary>
    /// Stops the background log processing task gracefully.
    /// </summary>
    public async Task StopAsync()
    {
        if (_cts is null || _processingTask is null)
            throw new InvalidOperationException("Logging service not started.");

        try
        {
            var timeout = TimeSpan.FromSeconds(_options.StopTimeout);
            _writer.Complete();
            await _processingTask.WaitAsync(timeout, _cts.Token);
            await _cts.CancelAsync();
        }
        catch (OperationCanceledException)
        {
            // Ignore cancellation exceptions during shutdown
        }
        finally
        {
            _cts.Dispose();
            _cts = null;
            _processingTask = null;
        }
    }

    /// <summary>
    /// Applies any pending migrations to the logging database.
    /// </summary>
    public Task MigrateAsync(CancellationToken token = default)
    {
        var scope = _provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LoggingDbContext>();
        return context.Database.MigrateAsync(token);
    }

    /// <summary>
    /// Processes log entries from the channel and writes them to the database.
    /// </summary>
    private async Task ProcessLogQueueAsync()
    {
        if (_cts is null)
            throw new InvalidOperationException("Logging service not started.");

        var scope = _provider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<LoggingWriteContext>();
        await foreach (var entry in _reader.ReadAllAsync(_cts.Token))
        {
            await context.Logs.AddAsync(entry, _cts.Token);
            await context.SaveChangesAsync(_cts.Token);
        }
    }

    /// <summary>
    /// Disposes the service and its resources.
    /// </summary>
    public void Dispose()
    {
        _cts?.Dispose();
        GC.SuppressFinalize(this);
    }
}
