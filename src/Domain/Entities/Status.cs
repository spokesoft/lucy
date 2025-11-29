using Lucy.Domain.Enums;

namespace Lucy.Domain.Entities;

/// <summary>
/// A status entity.
/// </summary>
public class Status : DomainEntity<long>
{
    /// <summary>
    /// The ID of the project this status belongs to.
    /// </summary>
    public long ProjectId { get; private set; }

    /// <summary>
    /// The key of the status.
    /// </summary>
    public string Key { get; private set; }

    /// <summary>
    /// The order of the status.
    /// </summary>
    public int Order { get; private set; }

    /// <summary>
    /// The name of the status.
    /// </summary>
    public string? Name { get; private set; }

    /// <summary>
    /// The description of the status.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// The color of the status.
    /// </summary>
    public Color Color { get; private set; } = Color.Gray;

    /// <summary>
    /// The project this status belongs to.
    /// </summary>
    public Project Project { get; private set; } = null!;

    /// <summary>
    /// The tickets associated with this status.
    /// </summary>
    public ICollection<Ticket> Tickets { get; private set; } = [];

    /// <summary>
    /// Parameterless constructor for EF Core.
    /// </summary>
    private Status()
    {
        Key = null!;
    }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    public Status(
        long projectId,
        string key,
        int order,
        string? name = null,
        string? description = null,
        Color? color = null)
    {
        ProjectId = projectId;
        Key = null!;

        UpdateKey(key);
        UpdateOrder(order);
        UpdateName(name);
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
            throw new ArgumentException("Status key cannot be null or empty.");

        if (!char.IsLetter(key[0]))
            throw new ArgumentException("Status key must start with a letter.");

        if (!key.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_'))
            throw new ArgumentException("Status key can only contain letters, numbers, underscores, and dashes.");

        if (key.Length > 15)
            throw new ArgumentException("Status key cannot be longer than 15 characters.", nameof(key));

        Key = key.ToUpperInvariant();
    }

    /// <summary>
    /// Updates the order of the status.
    /// </summary>
    /// <param name="order"></param>
    public void UpdateOrder(int order) => Order = order;

    /// <summary>
    /// Updates the name of the status.
    /// </summary>
    public void UpdateName(string? name)
    {
        if (name != null && name.Length > 50)
            throw new ArgumentException("Name cannot be longer than 50 characters.", nameof(name));

        Name = name;
    }

    /// <summary>
    /// Updates the description of the status.
    /// </summary>
    public void UpdateDescription(string? description)
    {
        if (description != null && description.Length > 100)
            throw new ArgumentException("Description cannot be longer than 100 characters.", nameof(description));

        Description = description;
    }

    /// <summary>
    /// Updates the color of the status.
    /// </summary>
    public void UpdateColor(Color color) => Color = color;
}
