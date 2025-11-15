using Lucy.Domain.Entities;

namespace Lucy.Domain.Tests;

public class TicketTests
{
    [Fact]
    public void Constructor_ShouldSetProperties_WhenValidArgumentsAreProvided()
    {
        var projectId = 1L;
        var statusId = 2L;
        var key = "PROJ-123";
        var title = "Test Ticket";
        var description = "This is a test ticket.";

        var ticket = new Ticket(projectId, statusId, key, title, description);

        Assert.Equal(projectId, ticket.ProjectId);
        Assert.Equal(statusId, ticket.StatusId);
        Assert.Equal(key, ticket.Key);
        Assert.Equal(title, ticket.Title);
        Assert.Equal(description, ticket.Description);
    }

    [Fact]
    public void Constructor_ShouldAllowNullDescription()
    {
        var projectId = 1L;
        var statusId = 2L;
        var key = "PROJ-123";
        var title = "Test Ticket";

        var ticket = new Ticket(projectId, statusId, key, title, null);

        Assert.Null(ticket.Description);
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenKeyIsNullOrEmpty()
    {
        var projectId = 1L;
        var statusId = 2L;
        var title = "Test Ticket";

        Assert.Throws<ArgumentException>(() => new Ticket(projectId, statusId, null!, title));
        Assert.Throws<ArgumentException>(() => new Ticket(projectId, statusId, string.Empty, title));
        Assert.Throws<ArgumentException>(() => new Ticket(projectId, statusId, "   ", title));
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenTitleIsNullOrEmpty()
    {
        var projectId = 1L;
        var statusId = 2L;
        var key = "PROJ-123";

        Assert.Throws<ArgumentException>(() => new Ticket(projectId, statusId, key, null!));
        Assert.Throws<ArgumentException>(() => new Ticket(projectId, statusId, key, string.Empty));
        Assert.Throws<ArgumentException>(() => new Ticket(projectId, statusId, key, "   "));
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenKeyExceeds20Characters()
    {
        var projectId = 1L;
        var statusId = 2L;
        var key = new string('A', 21);
        var title = "Test Ticket";

        Assert.Throws<ArgumentException>(() => new Ticket(projectId, statusId, key, title));
    }

    [Fact]
    public void Constructor_ShouldAcceptKeyWith20Characters()
    {
        var projectId = 1L;
        var statusId = 2L;
        var key = new string('A', 20);
        var title = "Test Ticket";

        var ticket = new Ticket(projectId, statusId, key, title);

        Assert.Equal(key, ticket.Key);
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenTitleExceeds200Characters()
    {
        var projectId = 1L;
        var statusId = 2L;
        var key = "PROJ-123";
        var title = new string('A', 201);

        Assert.Throws<ArgumentException>(() => new Ticket(projectId, statusId, key, title));
    }

    [Fact]
    public void Constructor_ShouldAcceptTitleWith200Characters()
    {
        var projectId = 1L;
        var statusId = 2L;
        var key = "PROJ-123";
        var title = new string('A', 200);

        var ticket = new Ticket(projectId, statusId, key, title);

        Assert.Equal(title, ticket.Title);
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenDescriptionExceeds5000Characters()
    {
        var projectId = 1L;
        var statusId = 2L;
        var key = "PROJ-123";
        var title = "Test Ticket";
        var description = new string('A', 5001);

        Assert.Throws<ArgumentException>(() => new Ticket(projectId, statusId, key, title, description));
    }

    [Fact]
    public void Constructor_ShouldAcceptDescriptionWith5000Characters()
    {
        var projectId = 1L;
        var statusId = 2L;
        var key = "PROJ-123";
        var title = "Test Ticket";
        var description = new string('A', 5000);

        var ticket = new Ticket(projectId, statusId, key, title, description);

        Assert.Equal(description, ticket.Description);
    }

    [Fact]
    public void UpdateKey_ShouldUpdateKey()
    {
        var ticket = new Ticket(1L, 2L, "PROJ-123", "Test Ticket");

        ticket.UpdateKey("PROJ-456");

        Assert.Equal("PROJ-456", ticket.Key);
    }

    [Fact]
    public void UpdateKey_ShouldThrowException_WhenKeyIsNullOrEmpty()
    {
        var ticket = new Ticket(1L, 2L, "PROJ-123", "Test Ticket");

        Assert.Throws<ArgumentException>(() => ticket.UpdateKey(null!));
        Assert.Throws<ArgumentException>(() => ticket.UpdateKey(string.Empty));
        Assert.Throws<ArgumentException>(() => ticket.UpdateKey("   "));
    }

    [Fact]
    public void UpdateKey_ShouldThrowException_WhenKeyExceeds20Characters()
    {
        var ticket = new Ticket(1L, 2L, "PROJ-123", "Test Ticket");
        var longKey = new string('A', 21);

        Assert.Throws<ArgumentException>(() => ticket.UpdateKey(longKey));
    }

    [Fact]
    public void UpdateKey_ShouldAcceptKeyWith20Characters()
    {
        var ticket = new Ticket(1L, 2L, "PROJ-123", "Test Ticket");
        var maxKey = new string('A', 20);

        ticket.UpdateKey(maxKey);

        Assert.Equal(maxKey, ticket.Key);
    }

    [Fact]
    public void UpdateTitle_ShouldUpdateTitle()
    {
        var ticket = new Ticket(1L, 2L, "PROJ-123", "Original Title");

        ticket.UpdateTitle("Updated Title");

        Assert.Equal("Updated Title", ticket.Title);
    }

    [Fact]
    public void UpdateTitle_ShouldThrowException_WhenTitleIsNullOrEmpty()
    {
        var ticket = new Ticket(1L, 2L, "PROJ-123", "Test Ticket");

        Assert.Throws<ArgumentException>(() => ticket.UpdateTitle(null!));
        Assert.Throws<ArgumentException>(() => ticket.UpdateTitle(string.Empty));
        Assert.Throws<ArgumentException>(() => ticket.UpdateTitle("   "));
    }

    [Fact]
    public void UpdateTitle_ShouldThrowException_WhenTitleExceeds200Characters()
    {
        var ticket = new Ticket(1L, 2L, "PROJ-123", "Test Ticket");
        var longTitle = new string('A', 201);

        Assert.Throws<ArgumentException>(() => ticket.UpdateTitle(longTitle));
    }

    [Fact]
    public void UpdateTitle_ShouldAcceptTitleWith200Characters()
    {
        var ticket = new Ticket(1L, 2L, "PROJ-123", "Test Ticket");
        var maxTitle = new string('A', 200);

        ticket.UpdateTitle(maxTitle);

        Assert.Equal(maxTitle, ticket.Title);
    }

    [Fact]
    public void UpdateDescription_ShouldUpdateDescription()
    {
        var ticket = new Ticket(1L, 2L, "PROJ-123", "Test Ticket", "Original Description");

        ticket.UpdateDescription("Updated Description");

        Assert.Equal("Updated Description", ticket.Description);
    }

    [Fact]
    public void UpdateDescription_ShouldAllowNull()
    {
        var ticket = new Ticket(1L, 2L, "PROJ-123", "Test Ticket", "Original Description");

        ticket.UpdateDescription(null);

        Assert.Null(ticket.Description);
    }

    [Fact]
    public void UpdateDescription_ShouldThrowException_WhenDescriptionExceeds5000Characters()
    {
        var ticket = new Ticket(1L, 2L, "PROJ-123", "Test Ticket");
        var longDescription = new string('A', 5001);

        Assert.Throws<ArgumentException>(() => ticket.UpdateDescription(longDescription));
    }

    [Fact]
    public void UpdateDescription_ShouldAcceptDescriptionWith5000Characters()
    {
        var ticket = new Ticket(1L, 2L, "PROJ-123", "Test Ticket");
        var maxDescription = new string('A', 5000);

        ticket.UpdateDescription(maxDescription);

        Assert.Equal(maxDescription, ticket.Description);
    }

    [Fact]
    public void UpdateStatus_ShouldUpdateStatusId()
    {
        var ticket = new Ticket(1L, 2L, "PROJ-123", "Test Ticket");

        ticket.UpdateStatus(3L);

        Assert.Equal(3L, ticket.StatusId);
    }

    [Fact]
    public void UpdateStatus_ShouldAllowMultipleUpdates()
    {
        var ticket = new Ticket(1L, 2L, "PROJ-123", "Test Ticket");

        ticket.UpdateStatus(3L);
        Assert.Equal(3L, ticket.StatusId);

        ticket.UpdateStatus(4L);
        Assert.Equal(4L, ticket.StatusId);

        ticket.UpdateStatus(2L);
        Assert.Equal(2L, ticket.StatusId);
    }
}
