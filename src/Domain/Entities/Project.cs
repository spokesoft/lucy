using Lucy.Domain.Enums;

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
    /// The sequences associated with the project.
    /// </summary>
    public ICollection<Sequence> Sequences { get; private set; } = [];

    /// <summary>
    /// The statuses associated with the project.
    /// </summary>
    public ICollection<Status> Statuses { get; private set; } = [];

    /// <summary>
    /// The tickets associated with the project.
    /// </summary>
    public ICollection<Ticket> Tickets { get; private set; } = [];

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

        Sequences = [
            new Sequence(SequenceType.Ticket, Id, template: Key + "-{0}"),
            new Sequence(SequenceType.Iteration, Id, template: Key + "-S{0}")
        ];

        Statuses = [
            new Status(Id, "TODO",
                order: 1,
                name: "To Do",
                description: "Tasks that need to be done",
                color: StatusColor.Gray),

            new Status(Id, "IN-PROGRESS",
                order: 2,
                name: "In Progress",
                description: "Tasks that are in progress",
                color: StatusColor.Blue),

            new Status(Id, "DONE",
                order: 3,
                name: "Done",
                description: "Completed tasks",
                color: StatusColor.Green)
        ];
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

        if (key.Length > 10)
            throw new ArgumentException("Project key cannot be longer than 10 characters.", nameof(key));

        Key = key.ToUpperInvariant();

        if (Sequences is not null && Sequences.Count > 0)
        {
            foreach (var sequence in Sequences)
            {
                if (sequence.Type == SequenceType.Ticket)
                    sequence.UpdateTemplate(Key + "-{0}");
                else if (sequence.Type == SequenceType.Iteration)
                    sequence.UpdateTemplate(Key + "-S{0}");
            }
        }
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
