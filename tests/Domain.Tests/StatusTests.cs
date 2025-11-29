using Lucy.Domain.Entities;
using Lucy.Domain.Enums;

namespace Lucy.Domain.Tests;

public class StatusTests
{
    [Fact]
    public void Status_Constructor_ShouldSetProperties_WhenValidArgumentsAreProvided()
    {
        var projectId = 1L;
        var key = "test-key";
        var order = 1;
        var name = "Test Name";
        var description = "Test Description";
        var color = Color.Red;

        var status = new Status(projectId, key, order, name, description, color);

        Assert.Equal(projectId, status.ProjectId);
        Assert.Equal("TEST-KEY", status.Key);
        Assert.Equal(order, status.Order);
        Assert.Equal(name, status.Name);
        Assert.Equal(description, status.Description);
        Assert.Equal(color, status.Color);
    }

    [Fact]
    public void Status_Constructor_ShouldUseDefaultColor_WhenColorIsNotProvided()
    {
        var projectId = 1L;
        var key = "test-key";
        var order = 1;

        var status = new Status(projectId, key, order);

        Assert.Equal(Color.Gray, status.Color);
    }

    [Fact]
    public void Status_UpdateKey_ShouldUpdateKey_WhenValidKeyIsProvided()
    {
        var status = new Status(1L, "old-key", 1);
        var newKey = "new-key";

        status.UpdateKey(newKey);

        Assert.Equal("NEW-KEY", status.Key);
    }

    [Fact]
    public void Status_UpdateKey_ShouldThrowException_WhenKeyIsNull()
    {
        var status = new Status(1L, "old-key", 1);

        Assert.Throws<ArgumentException>(() => status.UpdateKey(null!));
    }

    [Fact]
    public void Status_UpdateKey_ShouldThrowException_WhenKeyIsEmpty()
    {
        var status = new Status(1L, "old-key", 1);

        Assert.Throws<ArgumentException>(() => status.UpdateKey(string.Empty));
    }

    [Fact]
    public void Status_UpdateKey_ShouldThrowException_WhenKeyIsWhitespace()
    {
        var status = new Status(1L, "old-key", 1);

        Assert.Throws<ArgumentException>(() => status.UpdateKey("   "));
    }

    [Fact]
    public void Status_UpdateKey_ShouldThrowException_WhenKeyDoesNotStartWithLetter()
    {
        var status = new Status(1L, "old-key", 1);

        Assert.Throws<ArgumentException>(() => status.UpdateKey("1key"));
    }

    [Fact]
    public void Status_UpdateKey_ShouldThrowException_WhenKeyContainsInvalidCharacters()
    {
        var status = new Status(1L, "old-key", 1);

        Assert.Throws<ArgumentException>(() => status.UpdateKey("key@invalid"));
    }

    [Fact]
    public void Status_UpdateKey_ShouldThrowException_WhenKeyIsTooLong()
    {
        var status = new Status(1L, "old-key", 1);
        var longKey = new string('a', 16);

        Assert.Throws<ArgumentException>(() => status.UpdateKey(longKey));
    }

    [Fact]
    public void Status_UpdateOrder_ShouldUpdateOrder()
    {
        var status = new Status(1L, "key", 1);
        var newOrder = 5;

        status.UpdateOrder(newOrder);

        Assert.Equal(newOrder, status.Order);
    }

    [Fact]
    public void Status_UpdateName_ShouldUpdateName_WhenValidNameIsProvided()
    {
        var status = new Status(1L, "key", 1);
        var newName = "New Name";

        status.UpdateName(newName);

        Assert.Equal(newName, status.Name);
    }

    [Fact]
    public void Status_UpdateName_ShouldThrowException_WhenNameIsTooLong()
    {
        var status = new Status(1L, "key", 1);
        var longName = new string('a', 51);

        Assert.Throws<ArgumentException>(() => status.UpdateName(longName));
    }

    [Fact]
    public void Status_UpdateDescription_ShouldUpdateDescription_WhenValidDescriptionIsProvided()
    {
        var status = new Status(1L, "key", 1);
        var newDescription = "New Description";

        status.UpdateDescription(newDescription);

        Assert.Equal(newDescription, status.Description);
    }

    [Fact]
    public void Status_UpdateDescription_ShouldThrowException_WhenDescriptionIsTooLong()
    {
        var status = new Status(1L, "key", 1);
        var longDescription = new string('a', 101);

        Assert.Throws<ArgumentException>(() => status.UpdateDescription(longDescription));
    }

    [Fact]
    public void Status_UpdateColor_ShouldUpdateColor()
    {
        var status = new Status(1L, "key", 1);
        var newColor = Color.Blue;

        status.UpdateColor(newColor);

        Assert.Equal(newColor, status.Color);
    }
}
