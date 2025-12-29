namespace Lucy.Application.Common.Filters;

/// <summary>
/// Represents a group of filter nodes combined by a logical operator.
/// </summary>
/// <param name="Operator">The logical operator (AND/OR).</param>
/// <param name="Nodes">The collection of child nodes.</param>
public record FilterGroup(
    LogicOperator Operator,
    IEnumerable<FilterNode> Nodes
) : FilterNode;
