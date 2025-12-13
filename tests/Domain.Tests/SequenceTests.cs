using Lucy.Domain.Entities;
using Lucy.Domain.Enums;

namespace Lucy.Domain.Tests;

/// <summary>
/// Tests for the Sequence domain entity.
/// </summary>
public class SequenceTests
{
    [Fact]
    public void Constructor_ShouldSetProperties_WhenValidArgumentsAreProvided()
    {
        // Arrange
        var type = SequenceType.Ticket;
        var projectId = 1L;
        var value = 5;
        var template = "TEST-{0}";

        // Act
        var sequence = new Sequence(type, projectId, value, template);

        // Assert
        Assert.Equal(type, sequence.Type);
        Assert.Equal(projectId, sequence.ProjectId);
        Assert.Equal(value, sequence.Value);
        Assert.Equal(template, sequence.Template);
    }

    [Fact]
    public void Constructor_ShouldUseDefaultValues_WhenOptionalArgumentsAreOmitted()
    {
        // Arrange
        var type = SequenceType.Iteration;
        var projectId = 2L;

        // Act
        var sequence = new Sequence(type, projectId);

        // Assert
        Assert.Equal(type, sequence.Type);
        Assert.Equal(projectId, sequence.ProjectId);
        Assert.Equal(0, sequence.Value);
        Assert.Equal("{0}", sequence.Template);
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenTemplateIsNull()
    {
        // Arrange
        var type = SequenceType.Ticket;
        var projectId = 1L;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Sequence(type, projectId, 0, null!));
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenTemplateIsEmpty()
    {
        // Arrange
        var type = SequenceType.Ticket;
        var projectId = 1L;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new Sequence(type, projectId, 0, string.Empty));
        Assert.Throws<ArgumentException>(() => new Sequence(type, projectId, 0, "   "));
    }

    [Fact]
    public void UpdateTemplate_ShouldUpdateTemplate_WhenValidTemplateIsProvided()
    {
        // Arrange
        var sequence = new Sequence(SequenceType.Ticket, 1L);
        var newTemplate = "PROJECT-{0}";

        // Act
        sequence.UpdateTemplate(newTemplate);

        // Assert
        Assert.Equal(newTemplate, sequence.Template);
    }

    [Fact]
    public void UpdateTemplate_ShouldThrowException_WhenTemplateIsNullOrEmpty()
    {
        // Arrange
        var sequence = new Sequence(SequenceType.Ticket, 1L);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => sequence.UpdateTemplate(null!));
        Assert.Throws<ArgumentException>(() => sequence.UpdateTemplate(string.Empty));
        Assert.Throws<ArgumentException>(() => sequence.UpdateTemplate("   "));
    }

    [Fact]
    public void Next_ShouldIncrementValueAndReturnFormattedString()
    {
        // Arrange
        var sequence = new Sequence(SequenceType.Ticket, 1L, 0, "TEST-{0}");

        // Act
        var result = sequence.Next();

        // Assert
        Assert.Equal("TEST-1", result);
        Assert.Equal(1, sequence.Value);
    }

    [Fact]
    public void Next_ShouldIncrementValueMultipleTimes()
    {
        // Arrange
        var sequence = new Sequence(SequenceType.Ticket, 1L, 0, "PROJ-{0}");

        // Act
        var result1 = sequence.Next();
        var result2 = sequence.Next();
        var result3 = sequence.Next();

        // Assert
        Assert.Equal("PROJ-1", result1);
        Assert.Equal("PROJ-2", result2);
        Assert.Equal("PROJ-3", result3);
        Assert.Equal(3, sequence.Value);
    }

    [Fact]
    public void Next_ShouldContinueFromInitialValue()
    {
        // Arrange
        var sequence = new Sequence(SequenceType.Iteration, 1L, 10, "SPRINT-{0}");

        // Act
        var result = sequence.Next();

        // Assert
        Assert.Equal("SPRINT-11", result);
        Assert.Equal(11, sequence.Value);
    }

    [Fact]
    public void PreviewNext_ShouldReturnNextValueWithoutIncrementing()
    {
        // Arrange
        var sequence = new Sequence(SequenceType.Ticket, 1L, 5, "ISSUE-{0}");

        // Act
        var preview = sequence.PreviewNext();

        // Assert
        Assert.Equal("ISSUE-6", preview);
        Assert.Equal(5, sequence.Value); // Value should not change
    }

    [Fact]
    public void PreviewNext_ShouldReturnSameValueWhenCalledMultipleTimes()
    {
        // Arrange
        var sequence = new Sequence(SequenceType.Ticket, 1L, 3, "TASK-{0}");

        // Act
        var preview1 = sequence.PreviewNext();
        var preview2 = sequence.PreviewNext();
        var preview3 = sequence.PreviewNext();

        // Assert
        Assert.Equal("TASK-4", preview1);
        Assert.Equal("TASK-4", preview2);
        Assert.Equal("TASK-4", preview3);
        Assert.Equal(3, sequence.Value);
    }

    [Fact]
    public void Next_AndPreviewNext_ShouldWorkTogether()
    {
        // Arrange
        var sequence = new Sequence(SequenceType.Ticket, 1L, 0, "BUG-{0}");

        // Act
        var preview1 = sequence.PreviewNext();
        var next1 = sequence.Next();
        var preview2 = sequence.PreviewNext();
        var next2 = sequence.Next();

        // Assert
        Assert.Equal("BUG-1", preview1);
        Assert.Equal("BUG-1", next1);
        Assert.Equal("BUG-2", preview2);
        Assert.Equal("BUG-2", next2);
        Assert.Equal(2, sequence.Value);
    }

    [Fact]
    public void Template_ShouldSupportDifferentFormats()
    {
        // Arrange
        var sequence1 = new Sequence(SequenceType.Ticket, 1L, 0, "{0}");
        var sequence2 = new Sequence(SequenceType.Ticket, 1L, 0, "PREFIX-{0}-SUFFIX");
        var sequence3 = new Sequence(SequenceType.Ticket, 1L, 0, "{0:D5}");

        // Act & Assert
        Assert.Equal("1", sequence1.Next());
        Assert.Equal("PREFIX-1-SUFFIX", sequence2.Next());
        Assert.Equal("00001", sequence3.Next());
    }

    [Fact]
    public void Type_ShouldDistinguishBetweenSequenceTypes()
    {
        // Arrange
        var ticketSequence = new Sequence(SequenceType.Ticket, 1L);
        var iterationSequence = new Sequence(SequenceType.Iteration, 1L);

        // Act & Assert
        Assert.Equal(SequenceType.Ticket, ticketSequence.Type);
        Assert.Equal(SequenceType.Iteration, iterationSequence.Type);
    }
}
