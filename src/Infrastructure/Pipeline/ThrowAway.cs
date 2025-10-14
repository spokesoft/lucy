namespace Lucy.Infrastructure.Pipeline;

/// <summary>
/// A struct used to indicate that a pipeline does not require input or output.
/// </summary>
public readonly struct ThrowAway
{
    public static readonly ThrowAway Value = new();
}
