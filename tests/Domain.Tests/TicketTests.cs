using Lucy.Domain.Entities;

namespace Lucy.Domain.Tests;

/// <summary>
/// Tests for the Ticket domain entity.
/// </summary>
public class TicketTests
{
    [Fact]
    public void Constructor_ShouldSetProperties_WhenValidArgumentsAreProvided()
    {
        // Arrange
        var projectId = 1L;
        var statusId = 2L;
        var key = "PROJ-123";
        var title = "Test Ticket";
        var description = "This is a test ticket.";

        // Act
        var ticket = new Ticket(projectId, statusId, key, 123, title, description);

        // Assert
        Assert.Equal(projectId, ticket.ProjectId);
        Assert.Equal(statusId, ticket.StatusId);
        Assert.Equal(key, ticket.Key);
        Assert.Equal(title, ticket.Title);
        Assert.Equal(description, ticket.Description);
    }

    [Fact]
    public void Constructor_ShouldAllowNullDescription()
    {
        // Arrange
        var projectId = 1L;
        var statusId = 2L;
        var key = "PROJ-123";
        var title = "Test Ticket";

        // Act
        var ticket = new Ticket(projectId, statusId, key, 123, title, null);

        // Assert
        Assert.Null(ticket.Description);
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenKeyIsNull()
    {
        // Arrange
        var projectId = 1L;
        var statusId = 2L;
        var title = "Test Ticket";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Ticket(projectId, statusId, null!, 123, title));
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenKeyIsEmpty()
    {
        // Arrange
        var projectId = 1L;
        var statusId = 2L;
        var title = "Test Ticket";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Ticket(projectId, statusId, string.Empty, 123, title));
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenKeyIsWhitespace()
    {
        // Arrange
        var projectId = 1L;
        var statusId = 2L;
        var title = "Test Ticket";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Ticket(projectId, statusId, "   ", 123, title));
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenTitleIsNull()
    {
        // Arrange
        var projectId = 1L;
        var statusId = 2L;
        var key = "PROJ-123";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Ticket(projectId, statusId, key, 123, null!));
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenTitleIsEmpty()
    {
        // Arrange
        var projectId = 1L;
        var statusId = 2L;
        var key = "PROJ-123";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Ticket(projectId, statusId, key, 123, string.Empty));
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenTitleIsWhitespace()
    {
        // Arrange
        var projectId = 1L;
        var statusId = 2L;
        var key = "PROJ-123";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Ticket(projectId, statusId, key, 123, "   "));
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenKeyExceeds20Characters()
    {
        // Arrange
        var projectId = 1L;
        var statusId = 2L;
        var key = new string('A', 21);
        var title = "Test Ticket";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Ticket(projectId, statusId, key, 123, title));
    }

    [Fact]
    public void Constructor_ShouldAcceptKeyWith20Characters()
    {
        // Arrange
        var projectId = 1L;
        var statusId = 2L;
        var key = new string('A', 20);
        var title = "Test Ticket";

        // Act
        var ticket = new Ticket(projectId, statusId, key, 123, title);

        // Assert
        Assert.Equal(key, ticket.Key);
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenTitleExceeds200Characters()
    {
        // Arrange
        var projectId = 1L;
        var statusId = 2L;
        var key = "PROJ-123";
        var title = new string('A', 201);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Ticket(projectId, statusId, key, 123, title));
    }

    [Fact]
    public void Constructor_ShouldAcceptTitleWith200Characters()
    {
        // Arrange
        var projectId = 1L;
        var statusId = 2L;
        var key = "PROJ-123";
        var title = new string('A', 200);

        // Act
        var ticket = new Ticket(projectId, statusId, key, 123, title);

        // Assert
        Assert.Equal(title, ticket.Title);
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenDescriptionExceeds5000Characters()
    {
        // Arrange
        var projectId = 1L;
        var statusId = 2L;
        var key = "PROJ-123";
        var title = "Test Ticket";
        var description = new string('A', 5001);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Ticket(projectId, statusId, key, 123, title, description));
    }

    [Fact]
    public void Constructor_ShouldAcceptDescriptionWith5000Characters()
    {
        // Arrange
        var projectId = 1L;
        var statusId = 2L;
        var key = "PROJ-123";
        var title = "Test Ticket";
        var description = new string('A', 5000);

        // Act
        var ticket = new Ticket(projectId, statusId, key, 123, title, description);

        // Assert
        Assert.Equal(description, ticket.Description);
    }

    [Fact]
    public void UpdateKey_ShouldUpdateKey()
    {
        // Arrange
        var ticket = new Ticket(1L, 2L, "PROJ-123", 123, "Test Ticket");

        // Act
        ticket.UpdateKey("PROJ-456");

        // Assert
        Assert.Equal("PROJ-456", ticket.Key);
    }

    [Fact]
    public void UpdateKey_ShouldThrowException_WhenKeyIsNull()
    {
        // Arrange
        var ticket = new Ticket(1L, 2L, "PROJ-123", 123, "Test Ticket");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => ticket.UpdateKey(null!));
    }

    [Fact]
    public void UpdateKey_ShouldThrowException_WhenKeyIsEmpty()
    {
        // Arrange
        var ticket = new Ticket(1L, 2L, "PROJ-123", 123, "Test Ticket");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => ticket.UpdateKey(string.Empty));
    }

    [Fact]
    public void UpdateKey_ShouldThrowException_WhenKeyIsWhitespace()
    {
        // Arrange
        var ticket = new Ticket(1L, 2L, "PROJ-123", 123, "Test Ticket");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => ticket.UpdateKey("   "));
    }

    [Fact]
    public void UpdateKey_ShouldThrowException_WhenKeyExceeds20Characters()
    {
        // Arrange
        var ticket = new Ticket(1L, 2L, "PROJ-123", 123, "Test Ticket");
        var longKey = new string('A', 21);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => ticket.UpdateKey(longKey));
    }

    [Fact]
    public void UpdateKey_ShouldAcceptKeyWith20Characters()
    {
        // Arrange
        var ticket = new Ticket(1L, 2L, "PROJ-123", 123, "Test Ticket");
        var maxKey = new string('A', 20);

        // Act
        ticket.UpdateKey(maxKey);

        // Assert
        Assert.Equal(maxKey, ticket.Key);
    }

    [Fact]
    public void UpdateTitle_ShouldUpdateTitle()
    {
        // Arrange
        var ticket = new Ticket(1L, 2L, "PROJ-123", 123, "Original Title");

        // Act
        ticket.UpdateTitle("Updated Title");

        // Assert
        Assert.Equal("Updated Title", ticket.Title);
    }

    [Fact]
    public void UpdateTitle_ShouldThrowException_WhenTitleIsNull()
    {
        // Arrange
        var ticket = new Ticket(1L, 2L, "PROJ-123", 1, "Test Ticket", null);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => ticket.UpdateTitle(null!));
    }

    [Fact]
    public void UpdateTitle_ShouldThrowException_WhenTitleIsEmpty()
    {
        // Arrange
        var ticket = new Ticket(1L, 2L, "PROJ-123", 1, "Test Ticket", null);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => ticket.UpdateTitle(string.Empty));
    }

    [Fact]
    public void UpdateTitle_ShouldThrowException_WhenTitleIsWhitespace()
    {
        // Arrange
        var ticket = new Ticket(1L, 2L, "PROJ-123", 1, "Test Ticket", null);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => ticket.UpdateTitle("   "));
    }

    [Fact]
    public void UpdateTitle_ShouldThrowException_WhenTitleExceeds200Characters()
    {
        // Arrange
        var ticket = new Ticket(1L, 2L, "PROJ-123", 1, "Test Ticket", null);
        var longTitle = new string('A', 201);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => ticket.UpdateTitle(longTitle));
    }

    [Fact]
    public void UpdateTitle_ShouldAcceptTitleWith200Characters()
    {
        // Arrange
        var ticket = new Ticket(1L, 2L, "PROJ-123", 1, "Test Ticket", null);
        var maxTitle = new string('A', 200);

        // Act
        ticket.UpdateTitle(maxTitle);

        // Assert
        Assert.Equal(maxTitle, ticket.Title);
    }

    [Fact]
    public void UpdateDescription_ShouldUpdateDescription()
    {
        // Arrange
        var ticket = new Ticket(1L, 2L, "PROJ-123", 1, "Test Ticket", "Original Description");

        // Act
        ticket.UpdateDescription("Updated Description");

        // Assert
        Assert.Equal("Updated Description", ticket.Description);
    }

    [Fact]
    public void UpdateDescription_ShouldAllowNull()
    {
        // Arrange
        var ticket = new Ticket(1L, 2L, "PROJ-123", 1, "Test Ticket", "Original Description");

        // Act
        ticket.UpdateDescription(null);

        // Assert
        Assert.Null(ticket.Description);
    }

    [Fact]
    public void UpdateDescription_ShouldThrowException_WhenDescriptionExceeds5000Characters()
    {
        // Arrange
        var ticket = new Ticket(1L, 2L, "PROJ-123", 1, "Test Ticket", null);
        var longDescription = new string('A', 5001);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => ticket.UpdateDescription(longDescription));
    }

    [Fact]
    public void UpdateDescription_ShouldAcceptDescriptionWith5000Characters()
    {
        // Arrange
        var ticket = new Ticket(1L, 2L, "PROJ-123", 1, "Test Ticket", null);
        var maxDescription = new string('A', 5000);

        // Act
        ticket.UpdateDescription(maxDescription);

        // Assert
        Assert.Equal(maxDescription, ticket.Description);
    }

    [Fact]
    public void UpdateStatus_ShouldUpdateStatusId()
    {
        // Arrange
        var ticket = new Ticket(1L, 2L, "PROJ-123", 1, "Test Ticket", null);

        // Act
        ticket.UpdateStatus(3L);

        // Assert
        Assert.Equal(3L, ticket.StatusId);
    }

    [Fact]
    public void UpdateStatus_ShouldAllowMultipleUpdates()
    {
        // Arrange
        var ticket = new Ticket(1L, 2L, "PROJ-123", 1, "Test Ticket", null);

        // Act & Assert
        ticket.UpdateStatus(3L);
        Assert.Equal(3L, ticket.StatusId);

        ticket.UpdateStatus(4L);
        Assert.Equal(4L, ticket.StatusId);

        ticket.UpdateStatus(2L);
        Assert.Equal(2L, ticket.StatusId);
    }
}
