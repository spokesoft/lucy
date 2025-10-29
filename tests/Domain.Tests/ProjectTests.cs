using Lucy.Domain.Entities;

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
}
