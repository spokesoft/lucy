using Lucy.Domain.Entities;
using Lucy.Domain.Enums;

namespace Lucy.Domain.Tests;

/// <summary>
/// Tests for the Status domain entity.
/// </summary>
public class StatusTests
{
    [Fact]
    public void Constructor_ShouldSetProperties_WhenValidArgumentsAreProvided()
    {
        // Arrange
        var projectId = 1L;
        var key = "test-key";
        var order = 1;
        var name = "Test Name";
        var description = "Test Description";
        var color = Color.Red;

        // Act
        var status = new Status(projectId, key, order, name, description, color);

        // Assert
        Assert.Equal(projectId, status.ProjectId);
        Assert.Equal("TEST-KEY", status.Key);
        Assert.Equal(order, status.Order);
        Assert.Equal(name, status.Name);
        Assert.Equal(description, status.Description);
        Assert.Equal(color, status.Color);
    }

    [Fact]
    public void Constructor_ShouldUseDefaultColor_WhenColorIsNotProvided()
    {
        // Arrange
        var projectId = 1L;
        var key = "test-key";
        var order = 1;

        // Act
        var status = new Status(projectId, key, order);

        // Assert
        Assert.Equal(Color.Gray, status.Color);
    }

    [Fact]
    public void UpdateKey_ShouldUpdateKey_WhenValidKeyIsProvided()
    {
        // Arrange
        var status = new Status(1L, "old-key", 1);
        var newKey = "new-key";

        // Act
        status.UpdateKey(newKey);

        // Assert
        Assert.Equal("NEW-KEY", status.Key);
    }

    [Fact]
    public void UpdateKey_ShouldThrowException_WhenKeyIsNull()
    {
        // Arrange
        var status = new Status(1L, "old-key", 1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => status.UpdateKey(null!));
    }

    [Fact]
    public void UpdateKey_ShouldThrowException_WhenKeyIsEmpty()
    {
        // Arrange
        var status = new Status(1L, "old-key", 1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => status.UpdateKey(string.Empty));
    }

    [Fact]
    public void UpdateKey_ShouldThrowException_WhenKeyIsWhitespace()
    {
        // Arrange
        var status = new Status(1L, "old-key", 1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => status.UpdateKey("   "));
    }

    [Fact]
    public void UpdateKey_ShouldThrowException_WhenKeyDoesNotStartWithLetter()
    {
        // Arrange
        var status = new Status(1L, "old-key", 1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => status.UpdateKey("1key"));
    }

    [Fact]
    public void UpdateKey_ShouldThrowException_WhenKeyContainsInvalidCharacters()
    {
        // Arrange
        var status = new Status(1L, "old-key", 1);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => status.UpdateKey("key@invalid"));
    }

    [Fact]
    public void UpdateKey_ShouldThrowException_WhenKeyIsTooLong()
    {
        // Arrange
        var status = new Status(1L, "old-key", 1);
        var longKey = new string('a', 16);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => status.UpdateKey(longKey));
    }

    [Fact]
    public void UpdateOrder_ShouldUpdateOrder()
    {
        // Arrange
        var status = new Status(1L, "key", 1);
        var newOrder = 5;

        // Act
        status.UpdateOrder(newOrder);

        // Assert
        Assert.Equal(newOrder, status.Order);
    }

    [Fact]
    public void UpdateName_ShouldUpdateName_WhenValidNameIsProvided()
    {
        // Arrange
        var status = new Status(1L, "key", 1);
        var newName = "New Name";

        // Act
        status.UpdateName(newName);

        // Assert
        Assert.Equal(newName, status.Name);
    }

    [Fact]
    public void UpdateName_ShouldThrowException_WhenNameIsTooLong()
    {
        // Arrange
        var status = new Status(1L, "key", 1);
        var longName = new string('a', 51);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => status.UpdateName(longName));
    }

    [Fact]
    public void UpdateDescription_ShouldUpdateDescription_WhenValidDescriptionIsProvided()
    {
        // Arrange
        var status = new Status(1L, "key", 1);
        var newDescription = "New Description";

        // Act
        status.UpdateDescription(newDescription);

        // Assert
        Assert.Equal(newDescription, status.Description);
    }

    [Fact]
    public void UpdateDescription_ShouldThrowException_WhenDescriptionIsTooLong()
    {
        // Arrange
        var status = new Status(1L, "key", 1);
        var longDescription = new string('a', 101);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => status.UpdateDescription(longDescription));
    }

    [Fact]
    public void UpdateColor_ShouldUpdateColor()
    {
        // Arrange
        var status = new Status(1L, "key", 1);
        var newColor = Color.Blue;

        // Act
        status.UpdateColor(newColor);

        // Assert
        Assert.Equal(newColor, status.Color);
    }
}
