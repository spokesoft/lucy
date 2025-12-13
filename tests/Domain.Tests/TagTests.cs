using Lucy.Domain.Entities;
using Lucy.Domain.Enums;

namespace Lucy.Domain.Tests;

/// <summary>
/// Tests for the Tag domain entity.
/// </summary>
public class TagTests
{
    [Fact]
    public void Constructor_ShouldSetProperties_WhenValidArgumentsAreProvided()
    {
        // Arrange
        var projectId = 1L;
        var key = "test-key";
        var label = "Test Label";
        var description = "Test Description";
        var color = Color.Red;

        // Act
        var tag = new Tag(projectId, key, label, description, color);

        // Assert
        Assert.Equal(projectId, tag.ProjectId);
        Assert.Equal("TEST-KEY", tag.Key);
        Assert.Equal(label, tag.Label);
        Assert.Equal(description, tag.Description);
        Assert.Equal(color, tag.Color);
    }

    [Fact]
    public void Constructor_ShouldUseDefaultColor_WhenColorIsNotProvided()
    {
        // Arrange
        var projectId = 1L;
        var key = "test-key";

        // Act
        var tag = new Tag(projectId, key);

        // Assert
        Assert.Equal(Color.Gray, tag.Color);
    }

    [Fact]
    public void UpdateKey_ShouldUpdateKey_WhenValidKeyIsProvided()
    {
        // Arrange
        var tag = new Tag(1L, "old-key");
        var newKey = "new-key";

        // Act
        tag.UpdateKey(newKey);

        // Assert
        Assert.Equal("NEW-KEY", tag.Key);
    }

    [Fact]
    public void UpdateKey_ShouldThrowException_WhenKeyIsNull()
    {
        // Arrange
        var tag = new Tag(1L, "old-key");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => tag.UpdateKey(null!));
    }

    [Fact]
    public void UpdateKey_ShouldThrowException_WhenKeyIsEmpty()
    {
        // Arrange
        var tag = new Tag(1L, "old-key");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => tag.UpdateKey(string.Empty));
    }

    [Fact]
    public void UpdateKey_ShouldThrowException_WhenKeyIsWhitespace()
    {
        // Arrange
        var tag = new Tag(1L, "old-key");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => tag.UpdateKey("   "));
    }

    [Fact]
    public void UpdateKey_ShouldThrowException_WhenKeyDoesNotStartWithLetter()
    {
        // Arrange
        var tag = new Tag(1L, "old-key");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => tag.UpdateKey("1key"));
    }

    [Fact]
    public void UpdateKey_ShouldThrowException_WhenKeyContainsInvalidCharacters()
    {
        // Arrange
        var tag = new Tag(1L, "old-key");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => tag.UpdateKey("key@invalid"));
    }

    [Fact]
    public void UpdateKey_ShouldThrowException_WhenKeyIsTooLong()
    {
        // Arrange
        var tag = new Tag(1L, "old-key");
        var longKey = new string('a', 16);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => tag.UpdateKey(longKey));
    }

    [Fact]
    public void UpdateLabel_ShouldUpdateLabel_WhenValidLabelIsProvided()
    {
        // Arrange
        var tag = new Tag(1L, "key");
        var newLabel = "New Label";

        // Act
        tag.UpdateLabel(newLabel);

        // Assert
        Assert.Equal(newLabel, tag.Label);
    }

    [Fact]
    public void UpdateLabel_ShouldThrowException_WhenLabelIsTooLong()
    {
        // Arrange
        var tag = new Tag(1L, "key");
        var longLabel = new string('a', 51);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => tag.UpdateLabel(longLabel));
    }

    [Fact]
    public void UpdateDescription_ShouldUpdateDescription_WhenValidDescriptionIsProvided()
    {
        // Arrange
        var tag = new Tag(1L, "key");
        var newDescription = "New Description";

        // Act
        tag.UpdateDescription(newDescription);

        // Assert
        Assert.Equal(newDescription, tag.Description);
    }

    [Fact]
    public void UpdateDescription_ShouldThrowException_WhenDescriptionIsTooLong()
    {
        // Arrange
        var tag = new Tag(1L, "key");
        var longDescription = new string('a', 101);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => tag.UpdateDescription(longDescription));
    }

    [Fact]
    public void UpdateColor_ShouldUpdateColor()
    {
        // Arrange
        var tag = new Tag(1L, "key");
        var newColor = Color.Blue;

        // Act
        tag.UpdateColor(newColor);

        // Assert
        Assert.Equal(newColor, tag.Color);
    }
}
