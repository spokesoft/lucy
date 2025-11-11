using Lucy.Domain.Enums;

namespace Lucy.Domain.Entities;

/// <summary>
/// A sequence entity.
/// </summary>
public class Sequence : DomainEntity<long>
{
    public SequenceType Type { get; private set; }

    /// <summary>
    /// The value of the sequence.
    /// </summary>
    public int Value { get; private set; }

    /// <summary>
    /// The template for generating sequence values.
    /// </summary>
    public string Template { get; private set; }

    /// <summary>
    /// The ID of the project this sequence belongs to.
    /// </summary>
    public long ProjectId { get; private set; }

    /// <summary>
    /// The project this sequence belongs to.
    /// </summary>
    public Project Project { get; private set; } = null!;

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    public Sequence(
        SequenceType type,
        long projectId,
        int value = 0,
        string template = "{0}")
    {
        Template = null!;

        Type = type;
        Value = value;
        ProjectId = projectId;
        UpdateTemplate(template);
    }

    /// <summary>
    /// Updates the template for the sequence.
    /// </summary>
    /// <param name="template"></param>
    /// <exception cref="ArgumentException"></exception>
    public void UpdateTemplate(string template)
    {
        if (string.IsNullOrWhiteSpace(template))
            throw new ArgumentException("Template cannot be null or empty.");

        Template = template;
    }

    /// <summary>
    /// Increments the sequence value and returns the formatted result.
    /// </summary>
    public string Next()
    {
        Value++;
        return string.Format(Template, Value);
    }

    /// <summary>
    /// Previews the next sequence value without incrementing.
    /// </summary>
    public string PreviewNext()
    {
        return string.Format(Template, Value + 1);
    }
}
