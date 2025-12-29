namespace Lucy.Application.Common.Filters;

/// <summary>
/// Represents a single filter criterion.
/// </summary>
/// <typeparam name="TField">The enum type representing the fields.</typeparam>
/// <param name="Field">The field to filter on.</param>
/// <param name="Operator">The operator to apply.</param>
/// <param name="Value">The value to compare against.</param>
public record FilterCriterion<TField>(
    TField Field,
    FilterOperator Operator,
    object? Value
) : FilterNode where TField : Enum;
