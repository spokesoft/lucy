using System.Diagnostics;
using System.Threading.Channels;
using Lucy.Infrastructure.Logging.Database;
using Lucy.Infrastructure.Logging.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
    private readonly IServiceScope _scope = provider.CreateScope();

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
    /// Stopwatch to measure the duration the service has been running.
    /// </summary>
    private Stopwatch? _stopwatch;

    /// <summary>
    /// The background task processing log entries.
    /// </summary>
    private Task? _processingTask;

    /// <summary>
    /// A factory function to generate a final log message when stopping the service.
    /// </summary>
    private Func<int, long, string>? _finalMessageFactory = null;

    /// <summary>
    /// Starts the background log processing task.
    /// </summary>
    public void Start(CancellationToken? token = null)
    {
        _cts = token is not null
            ? CancellationTokenSource.CreateLinkedTokenSource(token.Value)
            : new CancellationTokenSource();

        _stopwatch = Stopwatch.StartNew();
        _processingTask = Task.Run(ProcessLogQueueAsync);
    }

    /// <summary>
    /// Stops the background log processing task gracefully.
    /// </summary>
    public async Task StopAsync(Func<int, long, string>? finalMessageFactory = null)
    {
        _finalMessageFactory = finalMessageFactory;

        if (_cts is null || _processingTask is null)
            throw new InvalidOperationException("Logging service not started.");

        try
        {
            var timeout = TimeSpan.FromSeconds(_options.StopTimeout);
            _writer.Complete();
            await _processingTask.WaitAsync(timeout, _cts.Token);
        }
        catch (OperationCanceledException)
        {
            // Ignore cancellation exceptions during shutdown
        }
        catch (Exception)
        {
            _cts.Cancel();
            throw;
        }
    }

    /// <summary>
    /// Processes log entries from the channel and writes them to the database.
    /// </summary>
    private async Task ProcessLogQueueAsync()
    {
        if (_cts is null || _stopwatch is null)
            throw new InvalidOperationException("Logging service not started.");

        var totalLogs = 0;
        var context = _scope.ServiceProvider.GetRequiredService<LoggingWriteContext>();
        await foreach (var entry in _reader.ReadAllAsync(_cts.Token))
        {
            _cts.Token.ThrowIfCancellationRequested();
            await context.Logs.AddAsync(entry, _cts.Token);
            if (totalLogs % _options.BatchSize == 0)
                await context.SaveChangesAsync(_cts.Token);
            totalLogs++;
        }

        await context.SaveChangesAsync(_cts.Token);
        _stopwatch.Stop();

        if (_finalMessageFactory is not null)
        {
            var elapsed = _stopwatch.ElapsedMilliseconds;
            var finalMessage = _finalMessageFactory.Invoke(totalLogs, elapsed);
            await context.Logs.AddAsync(new LogEntry
            {
                Category = "Summary",
                EventId = new EventId(0, "FinalLog"),
                Timestamp = DateTime.UtcNow,
                Level = LogLevel.Debug,
                Message = finalMessage
            }, _cts.Token);
            await context.SaveChangesAsync(_cts.Token);
        }
    }

    /// <summary>
    /// Disposes the service and its resources.
    /// </summary>
    public void Dispose()
    {
        if (_cts is not null)
        {
            if (!_cts.IsCancellationRequested)
                _cts.Cancel();
            _cts.Dispose();
        }
        _processingTask?.Wait();
        _scope.Dispose();
        GC.SuppressFinalize(this);
    }
}
