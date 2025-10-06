namespace Lucy.Domain.Entities;

/// <summary>
/// A project entity.
/// </summary>
public class Project : DomainEntity<long>
{
    public required string Key { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
}
