using System.Threading.Channels;

namespace Lucy.Infrastructure.Logging.Options;

/// <summary>
/// Configuration options for the logging channel.
/// </summary>
public sealed class LoggingChannelOptions
{
    /// <summary>
    /// The configuration section name.
    /// </summary>
    public const string SectionName = "Channel";

    /// <summary>
    /// The capacity of the logging channel.
    /// </summary>
    public int Capacity { get; set; } = 1000;

    /// <summary>
    /// The behavior when the channel is full.
    /// </summary>
    public BoundedChannelFullMode FullMode { get; set; } = BoundedChannelFullMode.DropOldest;
}
