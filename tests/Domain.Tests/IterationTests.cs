using Lucy.Domain.Entities;

namespace Lucy.Domain.Tests;

/// <summary>
/// Tests for the Iteration domain entity.
/// </summary>
public class IterationTests
{
    [Fact]
    public void Constructor_ShouldSetProperties_WhenValidArgumentsAreProvided()
    {
        // Arrange
        var projectId = 1L;
        var key = "ITER-1";
        var number = 1;
        var name = "Iteration 1";
        var description = "First iteration";
        var startDate = DateTime.UtcNow;
        var endDate = DateTime.UtcNow.AddDays(14);

        // Act
        var iteration = new Iteration(projectId, key, number, name, description, startDate, endDate);

        // Assert
        Assert.Equal(projectId, iteration.ProjectId);
        Assert.Equal(key, iteration.Key);
        Assert.Equal(number, iteration.Number);
        Assert.Equal(name, iteration.Name);
        Assert.Equal(description, iteration.Description);
        Assert.Equal(startDate, iteration.StartDate);
        Assert.Equal(endDate, iteration.EndDate);
    }

    [Fact]
    public void UpdateKey_ShouldUpdateKey()
    {
        // Arrange
        var iteration = new Iteration(1L, "ITER-1", 1, "Name", "Desc", null, null);
        var newKey = "ITER-2";

        // Act
        iteration.UpdateKey(newKey);

        // Assert
        Assert.Equal(newKey, iteration.Key);
    }

    [Fact]
    public void UpdateKey_ShouldThrowException_WhenKeyIsNull()
    {
        // Arrange
        var iteration = new Iteration(1L, "ITER-1", 1, "Name", "Desc", null, null);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => iteration.UpdateKey(null!));
    }

    [Fact]
    public void UpdateKey_ShouldThrowException_WhenKeyIsEmpty()
    {
        // Arrange
        var iteration = new Iteration(1L, "ITER-1", 1, "Name", "Desc", null, null);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => iteration.UpdateKey(string.Empty));
    }

    [Fact]
    public void UpdateKey_ShouldThrowException_WhenKeyIsWhitespace()
    {
        // Arrange
        var iteration = new Iteration(1L, "ITER-1", 1, "Name", "Desc", null, null);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => iteration.UpdateKey("   "));
    }

    [Fact]
    public void UpdateKey_ShouldThrowException_WhenKeyExceeds50Characters()
    {
        // Arrange
        var iteration = new Iteration(1L, "ITER-1", 1, "Name", "Desc", null, null);
        var longKey = new string('A', 51);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => iteration.UpdateKey(longKey));
    }

    [Fact]
    public void UpdateNumber_ShouldUpdateNumber()
    {
        // Arrange
        var iteration = new Iteration(1L, "ITER-1", 1, "Name", "Desc", null, null);
        var newNumber = 2;

        // Act
        iteration.UpdateNumber(newNumber);

        // Assert
        Assert.Equal(newNumber, iteration.Number);
    }

    [Fact]
    public void UpdateNumber_ShouldThrowException_WhenNumberIsZero()
    {
        // Arrange
        var iteration = new Iteration(1L, "ITER-1", 1, "Name", "Desc", null, null);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => iteration.UpdateNumber(0));
    }

    [Fact]
    public void UpdateNumber_ShouldThrowException_WhenNumberIsNegative()
    {
        // Arrange
        var iteration = new Iteration(1L, "ITER-1", 1, "Name", "Desc", null, null);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => iteration.UpdateNumber(-1));
    }

    [Fact]
    public void UpdateName_ShouldUpdateName()
    {
        // Arrange
        var iteration = new Iteration(1L, "ITER-1", 1, "Name", "Desc", null, null);
        var newName = "New Name";

        // Act
        iteration.UpdateName(newName);

        // Assert
        Assert.Equal(newName, iteration.Name);
    }

    [Fact]
    public void UpdateName_ShouldAllowNull()
    {
        // Arrange
        var iteration = new Iteration(1L, "ITER-1", 1, "Name", "Desc", null, null);

        // Act
        iteration.UpdateName(null);

        // Assert
        Assert.Null(iteration.Name);
    }

    [Fact]
    public void UpdateName_ShouldThrowException_WhenNameExceeds100Characters()
    {
        // Arrange
        var iteration = new Iteration(1L, "ITER-1", 1, "Name", "Desc", null, null);
        var longName = new string('A', 101);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => iteration.UpdateName(longName));
    }

    [Fact]
    public void UpdateDescription_ShouldUpdateDescription()
    {
        // Arrange
        var iteration = new Iteration(1L, "ITER-1", 1, "Name", "Desc", null, null);
        var newDescription = "New Description";

        // Act
        iteration.UpdateDescription(newDescription);

        // Assert
        Assert.Equal(newDescription, iteration.Description);
    }

    [Fact]
    public void UpdateDescription_ShouldAllowNull()
    {
        // Arrange
        var iteration = new Iteration(1L, "ITER-1", 1, "Name", "Desc", null, null);

        // Act
        iteration.UpdateDescription(null);

        // Assert
        Assert.Null(iteration.Description);
    }

    [Fact]
    public void UpdateDescription_ShouldThrowException_WhenDescriptionExceeds500Characters()
    {
        // Arrange
        var iteration = new Iteration(1L, "ITER-1", 1, "Name", "Desc", null, null);
        var longDescription = new string('A', 501);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => iteration.UpdateDescription(longDescription));
    }

    [Fact]
    public void UpdateStartDate_ShouldUpdateStartDate()
    {
        // Arrange
        var iteration = new Iteration(1L, "ITER-1", 1, "Name", "Desc", null, null);
        var newStartDate = DateTime.UtcNow;

        // Act
        iteration.UpdateStartDate(newStartDate);

        // Assert
        Assert.Equal(newStartDate, iteration.StartDate);
    }

    [Fact]
    public void UpdateStartDate_ShouldAllowNull()
    {
        // Arrange
        var iteration = new Iteration(1L, "ITER-1", 1, "Name", "Desc", DateTime.UtcNow, null);

        // Act
        iteration.UpdateStartDate(null);

        // Assert
        Assert.Null(iteration.StartDate);
    }

    [Fact]
    public void UpdateStartDate_ShouldThrowException_WhenStartDateIsAfterEndDate()
    {
        // Arrange
        var endDate = DateTime.UtcNow;
        var iteration = new Iteration(1L, "ITER-1", 1, "Name", "Desc", null, endDate);
        var invalidStartDate = endDate.AddDays(1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => iteration.UpdateStartDate(invalidStartDate));
    }

    [Fact]
    public void UpdateEndDate_ShouldUpdateEndDate()
    {
        // Arrange
        var iteration = new Iteration(1L, "ITER-1", 1, "Name", "Desc", null, null);
        var newEndDate = DateTime.UtcNow;

        // Act
        iteration.UpdateEndDate(newEndDate);

        // Assert
        Assert.Equal(newEndDate, iteration.EndDate);
    }

    [Fact]
    public void UpdateEndDate_ShouldAllowNull()
    {
        // Arrange
        var iteration = new Iteration(1L, "ITER-1", 1, "Name", "Desc", null, DateTime.UtcNow);

        // Act
        iteration.UpdateEndDate(null);

        // Assert
        Assert.Null(iteration.EndDate);
    }

    [Fact]
    public void UpdateEndDate_ShouldThrowException_WhenEndDateIsBeforeStartDate()
    {
        // Arrange
        var startDate = DateTime.UtcNow;
        var iteration = new Iteration(1L, "ITER-1", 1, "Name", "Desc", startDate, null);
        var invalidEndDate = startDate.AddDays(-1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => iteration.UpdateEndDate(invalidEndDate));
    }
}
