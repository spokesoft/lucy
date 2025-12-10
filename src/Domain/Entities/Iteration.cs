namespace Lucy.Domain.Entities;

/// <summary>
/// An iteration entity.
/// </summary>
public class Iteration : DomainEntity<long>
{
    /// <summary>
    /// The ID of the project this iteration belongs to.
    /// </summary>
    public long ProjectId { get; set; }

    /// <summary>
    /// The project this iteration belongs to.
    /// </summary>
    public Project Project { get; set; } = null!;

    /// <summary>
    /// The unique key of the iteration.
    /// </summary>
    public string Key { get; set; }

    /// <summary>
    /// The sequence number of the iteration.
    /// </summary>
    public int Number { get; private set; }

    /// <summary>
    /// The name of the iteration.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The description of the iteration.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The start date of the iteration.
    /// </summary>
    public DateTime? StartDate { get; set; }

    /// <summary>
    /// The end date of the iteration.
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// The tickets associated with the iteration.
    /// </summary>
    public ICollection<Ticket> Tickets { get; private set; } = [];

    /// <summary>
    /// Parameterless constructor for EF Core.
    /// </summary>
    private Iteration()
    {
        Key = null!;
    }

    /// <summary>
    /// Initializes a new instance of the class.
    /// </summary>
    public Iteration(
        long projectId,
        string key,
        int number,
        string? name,
        string? description,
        DateTime? startDate,
        DateTime? endDate)
    {
        ProjectId = projectId;
        Key = null!;

        UpdateKey(key);
        UpdateNumber(number);
        UpdateName(name);
        UpdateDescription(description);
        UpdateStartDate(startDate);
        UpdateEndDate(endDate);
    }

    /// <summary>
    /// Updates the key of the iteration.
    /// </summary>
    public void UpdateKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Project key cannot be null or whitespace.");

        if (key.Length > 50)
            throw new ArgumentException("Project key cannot exceed 50 characters.");

        Key = key;
    }

    /// <summary>
    /// Updates the number of the iteration.
    /// </summary>
    public void UpdateNumber(int number)
    {
        if (number <= 0)
            throw new ArgumentException("Iteration number must be a positive integer.");

        Number = number;
    }

    /// <summary>
    /// Updates the name of the iteration.
    /// </summary>
    public void UpdateName(string? name)
    {
        if (name is not null && name.Length > 100)
            throw new ArgumentException("Project name cannot exceed 100 characters.");

        Name = name;
    }

    /// <summary>
    /// Updates the description of the iteration.
    /// </summary>
    public void UpdateDescription(string? description)
    {
        if (description is not null && description.Length > 500)
            throw new ArgumentException("Project description cannot exceed 500 characters.");

        Description = description;
    }

    /// <summary>
    /// Updates the start date of the iteration.
    /// </summary>
    public void UpdateStartDate(DateTime? startDate)
    {
        if (EndDate.HasValue && startDate.HasValue && startDate > EndDate)
            throw new ArgumentException("Start date must be before end date.");

        StartDate = startDate;
    }

    /// <summary>
    /// Updates the end date of the iteration.
    /// </summary>
    public void UpdateEndDate(DateTime? endDate)
    {
        if (StartDate.HasValue && endDate.HasValue && StartDate > endDate)
            throw new ArgumentException("End date must be after start date.");

        EndDate = endDate;
    }
}
