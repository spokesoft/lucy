using Lucy.Domain.Enums;

namespace Lucy.Domain.Entities;

/// <summary>
/// A tag entity.
/// </summary>
public class Tag : DomainEntity<long>
{
    /// <summary>
    /// The ID of the project this tag belongs to.
    /// </summary>
    public long ProjectId { get; private set; }

    /// <summary>
    /// The key of the tag.
    /// </summary>
    public string Key { get; private set; }

    /// <summary>
    /// The name of the tag.
    /// </summary>
    public string? Label { get; private set; }

    /// <summary>
    /// The description of the tag.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// The color of the tag.
    /// </summary>
    public Color Color { get; private set; } = Color.Gray;

    /// <summary>
    /// The project this tag belongs to.
    /// </summary>
    public Project Project { get; private set; } = null!;

    /// <summary>
    /// The ticket tags associated with this tag.
    /// </summary>
    public ICollection<TicketTag> TicketTags { get; private set; } = [];

    /// <summary>
    /// Parameterless constructor for EF Core.
    /// </summary>
    private Tag()
    {
        Key = null!;
    }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    public Tag(
        long projectId,
        string key,
        string? label = null,
        string? description = null,
        Color? color = null)
    {
        ProjectId = projectId;
        Key = null!;

        UpdateKey(key);
        UpdateLabel(label);
        UpdateDescription(description);
        if (color.HasValue)
            UpdateColor(color.Value);
    }

    /// <summary>
    /// Updates the key of the status.
    /// </summary>
    public void UpdateKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Tag key cannot be null or empty.");

        if (!char.IsLetter(key[0]))
            throw new ArgumentException("Tag key must start with a letter.");

        if (!key.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_'))
            throw new ArgumentException("Tag key can only contain letters, numbers, underscores, and dashes.");

        if (key.Length > 15)
            throw new ArgumentException("Tag key cannot be longer than 15 characters.", nameof(key));

        Key = key.ToUpperInvariant();
    }

    /// <summary>
    /// Updates the name of the tag.
    /// </summary>
    public void UpdateLabel(string? label)
    {
        if (label != null && label.Length > 50)
            throw new ArgumentException("Label cannot be longer than 50 characters.", nameof(label));

        Label = label;
    }

    /// <summary>
    /// Updates the description of the tag.
    /// </summary>
    public void UpdateDescription(string? description)
    {
        if (description != null && description.Length > 100)
            throw new ArgumentException("Description cannot be longer than 100 characters.", nameof(description));

        Description = description;
    }

    /// <summary>
    /// Updates the color of the tag.
    /// </summary>
    public void UpdateColor(Color color) => Color = color;
}
