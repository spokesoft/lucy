using Lucy.Domain.Entities;
using Lucy.Domain.Enums;

namespace Lucy.Domain.Tests;

public class SequenceTests
{
    [Fact]
    public void Constructor_ShouldSetProperties_WhenValidArgumentsAreProvided()
    {
        var type = SequenceType.Ticket;
        var projectId = 1L;
        var value = 5;
        var template = "TEST-{0}";

        var sequence = new Sequence(type, projectId, value, template);

        Assert.Equal(type, sequence.Type);
        Assert.Equal(projectId, sequence.ProjectId);
        Assert.Equal(value, sequence.Value);
        Assert.Equal(template, sequence.Template);
    }

    [Fact]
    public void Constructor_ShouldUseDefaultValues_WhenOptionalArgumentsAreOmitted()
    {
        var type = SequenceType.Iteration;
        var projectId = 2L;

        var sequence = new Sequence(type, projectId);

        Assert.Equal(type, sequence.Type);
        Assert.Equal(projectId, sequence.ProjectId);
        Assert.Equal(0, sequence.Value);
        Assert.Equal("{0}", sequence.Template);
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenTemplateIsNull()
    {
        var type = SequenceType.Ticket;
        var projectId = 1L;

        Assert.Throws<ArgumentException>(() => new Sequence(type, projectId, 0, null!));
    }

    [Fact]
    public void Constructor_ShouldThrowException_WhenTemplateIsEmpty()
    {
        var type = SequenceType.Ticket;
        var projectId = 1L;

        Assert.Throws<ArgumentException>(() => new Sequence(type, projectId, 0, string.Empty));
        Assert.Throws<ArgumentException>(() => new Sequence(type, projectId, 0, "   "));
    }

    [Fact]
    public void UpdateTemplate_ShouldUpdateTemplate_WhenValidTemplateIsProvided()
    {
        var sequence = new Sequence(SequenceType.Ticket, 1L);
        var newTemplate = "PROJECT-{0}";

        sequence.UpdateTemplate(newTemplate);

        Assert.Equal(newTemplate, sequence.Template);
    }

    [Fact]
    public void UpdateTemplate_ShouldThrowException_WhenTemplateIsNullOrEmpty()
    {
        var sequence = new Sequence(SequenceType.Ticket, 1L);

        Assert.Throws<ArgumentException>(() => sequence.UpdateTemplate(null!));
        Assert.Throws<ArgumentException>(() => sequence.UpdateTemplate(string.Empty));
        Assert.Throws<ArgumentException>(() => sequence.UpdateTemplate("   "));
    }

    [Fact]
    public void Next_ShouldIncrementValueAndReturnFormattedString()
    {
        var sequence = new Sequence(SequenceType.Ticket, 1L, 0, "TEST-{0}");

        var result = sequence.Next();

        Assert.Equal("TEST-1", result);
        Assert.Equal(1, sequence.Value);
    }

    [Fact]
    public void Next_ShouldIncrementValueMultipleTimes()
    {
        var sequence = new Sequence(SequenceType.Ticket, 1L, 0, "PROJ-{0}");

        var result1 = sequence.Next();
        var result2 = sequence.Next();
        var result3 = sequence.Next();

        Assert.Equal("PROJ-1", result1);
        Assert.Equal("PROJ-2", result2);
        Assert.Equal("PROJ-3", result3);
        Assert.Equal(3, sequence.Value);
    }

    [Fact]
    public void Next_ShouldContinueFromInitialValue()
    {
        var sequence = new Sequence(SequenceType.Iteration, 1L, 10, "SPRINT-{0}");

        var result = sequence.Next();

        Assert.Equal("SPRINT-11", result);
        Assert.Equal(11, sequence.Value);
    }

    [Fact]
    public void PreviewNext_ShouldReturnNextValueWithoutIncrementing()
    {
        var sequence = new Sequence(SequenceType.Ticket, 1L, 5, "ISSUE-{0}");

        var preview = sequence.PreviewNext();

        Assert.Equal("ISSUE-6", preview);
        Assert.Equal(5, sequence.Value); // Value should not change
    }

    [Fact]
    public void PreviewNext_ShouldReturnSameValueWhenCalledMultipleTimes()
    {
        var sequence = new Sequence(SequenceType.Ticket, 1L, 3, "TASK-{0}");

        var preview1 = sequence.PreviewNext();
        var preview2 = sequence.PreviewNext();
        var preview3 = sequence.PreviewNext();

        Assert.Equal("TASK-4", preview1);
        Assert.Equal("TASK-4", preview2);
        Assert.Equal("TASK-4", preview3);
        Assert.Equal(3, sequence.Value);
    }

    [Fact]
    public void Next_AndPreviewNext_ShouldWorkTogether()
    {
        var sequence = new Sequence(SequenceType.Ticket, 1L, 0, "BUG-{0}");

        var preview1 = sequence.PreviewNext();
        var next1 = sequence.Next();
        var preview2 = sequence.PreviewNext();
        var next2 = sequence.Next();

        Assert.Equal("BUG-1", preview1);
        Assert.Equal("BUG-1", next1);
        Assert.Equal("BUG-2", preview2);
        Assert.Equal("BUG-2", next2);
        Assert.Equal(2, sequence.Value);
    }

    [Fact]
    public void Template_ShouldSupportDifferentFormats()
    {
        var sequence1 = new Sequence(SequenceType.Ticket, 1L, 0, "{0}");
        var sequence2 = new Sequence(SequenceType.Ticket, 1L, 0, "PREFIX-{0}-SUFFIX");
        var sequence3 = new Sequence(SequenceType.Ticket, 1L, 0, "{0:D5}");

        Assert.Equal("1", sequence1.Next());
        Assert.Equal("PREFIX-1-SUFFIX", sequence2.Next());
        Assert.Equal("00001", sequence3.Next());
    }

    [Fact]
    public void Type_ShouldDistinguishBetweenSequenceTypes()
    {
        var ticketSequence = new Sequence(SequenceType.Ticket, 1L);
        var iterationSequence = new Sequence(SequenceType.Iteration, 1L);

        Assert.Equal(SequenceType.Ticket, ticketSequence.Type);
        Assert.Equal(SequenceType.Iteration, iterationSequence.Type);
    }
}
