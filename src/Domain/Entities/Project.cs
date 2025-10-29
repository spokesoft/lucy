namespace Lucy.Domain.Entities;

/// <summary>
/// A project entity.
/// </summary>
public class Project : DomainEntity<long>
{
    /// <summary>
    /// The unique key of the project.
    /// </summary>
    public string Key { get; private set; }

    /// <summary>
    /// The name of the project.
    /// </summary>
    public string? Name { get; private set; }

    /// <summary>
    /// The description of the project.
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    public Project(
        string key,
        string? name = null,
        string? description = null)
    {
        Key = null!;

        UpdateKey(key);
        UpdateName(name);
        UpdateDescription(description);
    }

    /// <summary>
    /// Updates the project key.
    /// </summary>
    public void UpdateKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Project key cannot be null or empty.");

        if (!char.IsLetter(key[0]))
            throw new ArgumentException("Project key must start with a letter.");

        if (!key.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_'))
            throw new ArgumentException("Project key can only contain letters, numbers, underscores, and dashes.");

        Key = key.ToUpperInvariant();
    }

    /// <summary>
    /// Updates the project name.
    /// </summary>
    public void UpdateName(string? name)
    {
        if (name is not null && name.Length > 100)
            throw new ArgumentException("Project name cannot exceed 100 characters.");

        Name = name;
    }

    /// <summary>
    /// Updates the project description.
    /// </summary>
    public void UpdateDescription(string? description)
    {
        if (description is not null && description.Length > 500)
            throw new ArgumentException("Project description cannot exceed 500 characters.");

        Description = description;
    }
}
