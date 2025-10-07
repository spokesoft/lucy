namespace Lucy.Infrastructure.Mediation;

/// <summary>
/// Represents a void response for requests that do not return a value.
/// </summary>
public readonly struct VoidResponse
{
    public static readonly VoidResponse Value = new();
}
