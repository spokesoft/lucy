using System.Threading.Channels;
using Lucy.Infrastructure.Logging;
using Lucy.Infrastructure.Logging.Database;
using Microsoft.Extensions.Logging;

namespace Lucy.Tests.Infrastructure.Logging;

public class DatabaseLoggerTests
{
    [Fact]
    public async Task Log_ShouldWriteLogEntryToChannel_WhenEnabled()
    {
        // Arrange
        var channel = Channel.CreateUnbounded<LogEntry>();
        var logger = new DatabaseLogger("TestCategory", channel.Writer, LogLevel.Information);
        var logMessage = "This is a test message.";

        // Act
        logger.LogInformation(logMessage);

        // Assert
        var logEntry = await channel.Reader.ReadAsync();

        Assert.Equal(LogLevel.Information, logEntry.Level);
        Assert.Equal("TestCategory", logEntry.Category);
        Assert.Equal(logMessage, logEntry.Message);
        Assert.Null(logEntry.Exception);
        Assert.Equal(DateTime.UtcNow.Date, logEntry.Timestamp.Date);
    }

    [Fact]
    public void Log_ShouldNotWriteLogEntry_WhenLevelIsDisabled()
    {
        // Arrange
        var channel = Channel.CreateUnbounded<LogEntry>();
        var logger = new DatabaseLogger("TestCategory", channel.Writer, LogLevel.Warning);

        // Act
        logger.LogInformation("This should not be logged.");

        // Assert
        Assert.False(channel.Reader.TryRead(out _));
    }

    [Fact]
    public async Task Log_ShouldIncludeExceptionDetails_WhenExceptionIsProvided()
    {
        // Arrange
        var channel = Channel.CreateUnbounded<LogEntry>();
        var logger = new DatabaseLogger("TestCategory", channel.Writer, LogLevel.Error);
        var exception = new InvalidOperationException("Test exception");

        // Act
        logger.LogError(exception, "An error occurred.");

        // Assert
        var logEntry = await channel.Reader.ReadAsync();
        Assert.Equal(LogLevel.Error, logEntry.Level);
        Assert.Contains("InvalidOperationException", logEntry.Exception);
        Assert.Contains("Test exception", logEntry.Exception);
    }
}
