namespace Lucy.Application.Common.Filters;

/// <summary>
/// Defines the operators available for filtering.
/// </summary>
public enum FilterOperator
{
    /// <summary>
    /// Equality comparison.
    /// </summary>
    Equals,

    /// <summary>
    /// Inequality comparison.
    /// </summary>
    NotEquals,

    /// <summary>
    /// Greater than comparison.
    /// </summary>
    GreaterThan,

    /// <summary>
    /// Less than comparison.
    /// </summary>
    LessThan,

    /// <summary>
    /// Greater than or equal comparison.
    /// </summary>
    GreaterThanOrEqual,

    /// <summary>
    /// Less than or equal comparison.
    /// </summary>
    LessThanOrEqual,

    /// <summary>
    /// Contains comparison (for strings or collections).
    /// </summary>
    Contains,

    /// <summary>
    /// In comparison.
    In
}
