using Lucy.Domain.Entities;
using Lucy.Domain.Enums;

namespace Lucy.Domain.Tests;

public class ProjectTests
{
    [Fact]
    public void Constructor_ShouldSetProperties_WhenValidArgumentsAreProvided()
    {
        var key = "project1";
        var name = "Test Project";
        var description = "This is a test project.";

        var project = new Project(key, name, description);

        Assert.Equal(key.ToUpperInvariant(), project.Key);
        Assert.Equal(name, project.Name);
        Assert.Equal(description, project.Description);
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenKeyIsNullOrEmpty()
    {
        Assert.Throws<ArgumentException>(() => new Project(null!));
        Assert.Throws<ArgumentException>(() => new Project(string.Empty));
        Assert.Throws<ArgumentException>(() => new Project("   "));
    }

    [Fact]
    public void Constructor_ShouldConvertKeyToUpperCase()
    {
        var key = "project1";

        var project = new Project(key);

        Assert.Equal("PROJECT1", project.Key);
    }

    [Fact]
    public void Constructor_ShouldAllowNullNameAndDescription()
    {
        var key = "project1";

        var project = new Project(key, null, null);

        Assert.Null(project.Name);
        Assert.Null(project.Description);
    }

    [Fact]
    public void Constructor_ShouldCreateDefaultSequences()
    {
        var key = "TEST";

        var project = new Project(key);

        Assert.NotNull(project.Sequences);
        Assert.Equal(2, project.Sequences.Count);

        var ticketSequence = project.Sequences.FirstOrDefault(s => s.Type == SequenceType.Ticket);
        var iterationSequence = project.Sequences.FirstOrDefault(s => s.Type == SequenceType.Iteration);

        Assert.NotNull(ticketSequence);
        Assert.NotNull(iterationSequence);
        Assert.Equal("TEST-{0}", ticketSequence.Template);
        Assert.Equal("TEST-S{0}", iterationSequence.Template);
    }

    [Fact]
    public void Constructor_ShouldSetSequenceTemplatesBasedOnKey()
    {
        var key = "MYPROJ";

        var project = new Project(key);

        var ticketSequence = project.Sequences.First(s => s.Type == SequenceType.Ticket);
        var iterationSequence = project.Sequences.First(s => s.Type == SequenceType.Iteration);

        Assert.Equal("MYPROJ-{0}", ticketSequence.Template);
        Assert.Equal("MYPROJ-S{0}", iterationSequence.Template);
    }

    [Fact]
    public void UpdateKey_ShouldUpdateKeyAndSequenceTemplates()
    {
        var project = new Project("OLD");
        var oldTicketTemplate = project.Sequences.First(s => s.Type == SequenceType.Ticket).Template;
        var oldIterationTemplate = project.Sequences.First(s => s.Type == SequenceType.Iteration).Template;

        Assert.Equal("OLD-{0}", oldTicketTemplate);
        Assert.Equal("OLD-S{0}", oldIterationTemplate);

        project.UpdateKey("NEW");

        Assert.Equal("NEW", project.Key);

        var ticketSequence = project.Sequences.First(s => s.Type == SequenceType.Ticket);
        var iterationSequence = project.Sequences.First(s => s.Type == SequenceType.Iteration);

        Assert.Equal("NEW-{0}", ticketSequence.Template);
        Assert.Equal("NEW-S{0}", iterationSequence.Template);
    }

    [Fact]
    public void UpdateKey_ShouldThrowException_WhenKeyIsNullOrEmpty()
    {
        var project = new Project("TEST");

        Assert.Throws<ArgumentException>(() => project.UpdateKey(null!));
        Assert.Throws<ArgumentException>(() => project.UpdateKey(string.Empty));
        Assert.Throws<ArgumentException>(() => project.UpdateKey("   "));
    }

    [Fact]
    public void UpdateKey_ShouldThrowException_WhenKeyDoesNotStartWithLetter()
    {
        var project = new Project("TEST");

        Assert.Throws<ArgumentException>(() => project.UpdateKey("1TEST"));
        Assert.Throws<ArgumentException>(() => project.UpdateKey("-TEST"));
        Assert.Throws<ArgumentException>(() => project.UpdateKey("_TEST"));
    }

    [Fact]
    public void UpdateKey_ShouldThrowException_WhenKeyContainsInvalidCharacters()
    {
        var project = new Project("TEST");

        Assert.Throws<ArgumentException>(() => project.UpdateKey("TEST@"));
        Assert.Throws<ArgumentException>(() => project.UpdateKey("TEST#"));
        Assert.Throws<ArgumentException>(() => project.UpdateKey("TEST PROJECT"));
        Assert.Throws<ArgumentException>(() => project.UpdateKey("TEST.PROJ"));
    }

    [Fact]
    public void UpdateKey_ShouldAllowValidCharacters()
    {
        var project = new Project("TEST");

        project.UpdateKey("TEST-123");
        Assert.Equal("TEST-123", project.Key);

        project.UpdateKey("TEST_ABC");
        Assert.Equal("TEST_ABC", project.Key);

        project.UpdateKey("ABC123DEF");
        Assert.Equal("ABC123DEF", project.Key);
    }

    [Fact]
    public void UpdateKey_ShouldConvertToUpperCase()
    {
        var project = new Project("test");

        project.UpdateKey("newkey");

        Assert.Equal("NEWKEY", project.Key);
    }

    [Fact]
    public void UpdateName_ShouldUpdateName()
    {
        var project = new Project("TEST");

        project.UpdateName("New Name");

        Assert.Equal("New Name", project.Name);
    }

    [Fact]
    public void UpdateName_ShouldAllowNull()
    {
        var project = new Project("TEST", "Original Name");

        project.UpdateName(null);

        Assert.Null(project.Name);
    }

    [Fact]
    public void UpdateName_ShouldThrowException_WhenNameExceeds100Characters()
    {
        var project = new Project("TEST");
        var longName = new string('A', 101);

        Assert.Throws<ArgumentException>(() => project.UpdateName(longName));
    }

    [Fact]
    public void UpdateName_ShouldAcceptNameWith100Characters()
    {
        var project = new Project("TEST");
        var maxName = new string('A', 100);

        project.UpdateName(maxName);

        Assert.Equal(maxName, project.Name);
    }

    [Fact]
    public void UpdateDescription_ShouldUpdateDescription()
    {
        var project = new Project("TEST");

        project.UpdateDescription("New Description");

        Assert.Equal("New Description", project.Description);
    }

    [Fact]
    public void UpdateDescription_ShouldAllowNull()
    {
        var project = new Project("TEST", description: "Original Description");

        project.UpdateDescription(null);

        Assert.Null(project.Description);
    }

    [Fact]
    public void UpdateDescription_ShouldThrowException_WhenDescriptionExceeds500Characters()
    {
        var project = new Project("TEST");
        var longDescription = new string('A', 501);

        Assert.Throws<ArgumentException>(() => project.UpdateDescription(longDescription));
    }

    [Fact]
    public void UpdateDescription_ShouldAcceptDescriptionWith500Characters()
    {
        var project = new Project("TEST");
        var maxDescription = new string('A', 500);

        project.UpdateDescription(maxDescription);

        Assert.Equal(maxDescription, project.Description);
    }

    [Fact]
    public void Sequences_ShouldMaintainReferenceWhenKeyIsUpdated()
    {
        var project = new Project("ORIG");
        var originalTicketSequence = project.Sequences.First(s => s.Type == SequenceType.Ticket);
        var originalIterationSequence = project.Sequences.First(s => s.Type == SequenceType.Iteration);

        project.UpdateKey("UPDATED");

        var updatedTicketSequence = project.Sequences.First(s => s.Type == SequenceType.Ticket);
        var updatedIterationSequence = project.Sequences.First(s => s.Type == SequenceType.Iteration);

        // Same object references
        Assert.Same(originalTicketSequence, updatedTicketSequence);
        Assert.Same(originalIterationSequence, updatedIterationSequence);

        // But with updated templates
        Assert.Equal("UPDATED-{0}", updatedTicketSequence.Template);
        Assert.Equal("UPDATED-S{0}", updatedIterationSequence.Template);
    }
}
