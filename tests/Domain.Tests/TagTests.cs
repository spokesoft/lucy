using Lucy.Domain.Entities;
using Lucy.Domain.Enums;

namespace Lucy.Domain.Tests;

public class TagTests
{
    [Fact]
    public void Tag_Constructor_ShouldSetProperties_WhenValidArgumentsAreProvided()
    {
        var projectId = 1L;
        var key = "test-key";
        var label = "Test Label";
        var description = "Test Description";
        var color = Color.Red;

        var tag = new Tag(projectId, key, label, description, color);

        Assert.Equal(projectId, tag.ProjectId);
        Assert.Equal("TEST-KEY", tag.Key);
        Assert.Equal(label, tag.Label);
        Assert.Equal(description, tag.Description);
        Assert.Equal(color, tag.Color);
    }

    [Fact]
    public void Tag_Constructor_ShouldUseDefaultColor_WhenColorIsNotProvided()
    {
        var projectId = 1L;
        var key = "test-key";

        var tag = new Tag(projectId, key);

        Assert.Equal(Color.Gray, tag.Color);
    }

    [Fact]
    public void Tag_UpdateKey_ShouldUpdateKey_WhenValidKeyIsProvided()
    {
        var tag = new Tag(1L, "old-key");
        var newKey = "new-key";

        tag.UpdateKey(newKey);

        Assert.Equal("NEW-KEY", tag.Key);
    }

    [Fact]
    public void Tag_UpdateKey_ShouldThrowException_WhenKeyIsNull()
    {
        var tag = new Tag(1L, "old-key");

        Assert.Throws<ArgumentException>(() => tag.UpdateKey(null!));
    }

    [Fact]
    public void Tag_UpdateKey_ShouldThrowException_WhenKeyIsEmpty()
    {
        var tag = new Tag(1L, "old-key");

        Assert.Throws<ArgumentException>(() => tag.UpdateKey(string.Empty));
    }

    [Fact]
    public void Tag_UpdateKey_ShouldThrowException_WhenKeyIsWhitespace()
    {
        var tag = new Tag(1L, "old-key");

        Assert.Throws<ArgumentException>(() => tag.UpdateKey("   "));
    }

    [Fact]
    public void Tag_UpdateKey_ShouldThrowException_WhenKeyDoesNotStartWithLetter()
    {
        var tag = new Tag(1L, "old-key");

        Assert.Throws<ArgumentException>(() => tag.UpdateKey("1key"));
    }

    [Fact]
    public void Tag_UpdateKey_ShouldThrowException_WhenKeyContainsInvalidCharacters()
    {
        var tag = new Tag(1L, "old-key");

        Assert.Throws<ArgumentException>(() => tag.UpdateKey("key@invalid"));
    }

    [Fact]
    public void Tag_UpdateKey_ShouldThrowException_WhenKeyIsTooLong()
    {
        var tag = new Tag(1L, "old-key");
        var longKey = new string('a', 16);

        Assert.Throws<ArgumentException>(() => tag.UpdateKey(longKey));
    }

    [Fact]
    public void Tag_UpdateLabel_ShouldUpdateLabel_WhenValidLabelIsProvided()
    {
        var tag = new Tag(1L, "key");
        var newLabel = "New Label";

        tag.UpdateLabel(newLabel);

        Assert.Equal(newLabel, tag.Label);
    }

    [Fact]
    public void Tag_UpdateLabel_ShouldThrowException_WhenLabelIsTooLong()
    {
        var tag = new Tag(1L, "key");
        var longLabel = new string('a', 51);

        Assert.Throws<ArgumentException>(() => tag.UpdateLabel(longLabel));
    }

    [Fact]
    public void Tag_UpdateDescription_ShouldUpdateDescription_WhenValidDescriptionIsProvided()
    {
        var tag = new Tag(1L, "key");
        var newDescription = "New Description";

        tag.UpdateDescription(newDescription);

        Assert.Equal(newDescription, tag.Description);
    }

    [Fact]
    public void Tag_UpdateDescription_ShouldThrowException_WhenDescriptionIsTooLong()
    {
        var tag = new Tag(1L, "key");
        var longDescription = new string('a', 101);

        Assert.Throws<ArgumentException>(() => tag.UpdateDescription(longDescription));
    }

    [Fact]
    public void Tag_UpdateColor_ShouldUpdateColor()
    {
        var tag = new Tag(1L, "key");
        var newColor = Color.Blue;

        tag.UpdateColor(newColor);

        Assert.Equal(newColor, tag.Color);
    }
}
