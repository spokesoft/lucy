using Lucy.Domain.Entities;

namespace Lucy.Domain.Tests;

public class CommentTests
{
    // --- ProjectComment Tests ---

    [Fact]
    public void ProjectComment_Constructor_ShouldSetProperties_WhenValidArgumentsAreProvided()
    {
        var projectId = 1L;
        var content = "This is a project comment.";

        var comment = new ProjectComment(projectId, content);

        Assert.Equal(projectId, comment.ProjectId);
        Assert.Equal(content, comment.Content);
    }

    [Fact]
    public void ProjectComment_Constructor_ShouldThrowException_WhenContentIsNull()
    {
        var projectId = 1L;

        Assert.Throws<ArgumentException>(() => new ProjectComment(projectId, null!));
    }

    [Fact]
    public void ProjectComment_Constructor_ShouldThrowException_WhenContentIsEmpty()
    {
        var projectId = 1L;

        Assert.Throws<ArgumentException>(() => new ProjectComment(projectId, string.Empty));
    }

    [Fact]
    public void ProjectComment_Constructor_ShouldThrowException_WhenContentIsWhitespace()
    {
        var projectId = 1L;

        Assert.Throws<ArgumentException>(() => new ProjectComment(projectId, "   "));
    }

    [Fact]
    public void ProjectComment_Constructor_ShouldThrowException_WhenContentExceeds5000Characters()
    {
        var projectId = 1L;
        var longContent = new string('A', 5001);

        Assert.Throws<ArgumentException>(() => new ProjectComment(projectId, longContent));
    }

    [Fact]
    public void ProjectComment_Constructor_ShouldAcceptContentWith5000Characters()
    {
        var projectId = 1L;
        var maxContent = new string('A', 5000);

        var comment = new ProjectComment(projectId, maxContent);

        Assert.Equal(maxContent, comment.Content);
    }

    [Fact]
    public void ProjectComment_UpdateContent_ShouldUpdateContent()
    {
        var projectId = 1L;
        var comment = new ProjectComment(projectId, "Original content");

        comment.UpdateContent("Updated content");

        Assert.Equal("Updated content", comment.Content);
    }

    [Fact]
    public void ProjectComment_UpdateContent_ShouldThrowException_WhenContentIsNull()
    {
        var projectId = 1L;
        var comment = new ProjectComment(projectId, "Original content");

        Assert.Throws<ArgumentException>(() => comment.UpdateContent(null!));
    }

    [Fact]
    public void ProjectComment_UpdateContent_ShouldThrowException_WhenContentIsEmpty()
    {
        var projectId = 1L;
        var comment = new ProjectComment(projectId, "Original content");

        Assert.Throws<ArgumentException>(() => comment.UpdateContent(string.Empty));
    }

    [Fact]
    public void ProjectComment_UpdateContent_ShouldThrowException_WhenContentIsWhitespace()
    {
        var projectId = 1L;
        var comment = new ProjectComment(projectId, "Original content");

        Assert.Throws<ArgumentException>(() => comment.UpdateContent("   "));
    }

    [Fact]
    public void ProjectComment_UpdateContent_ShouldThrowException_WhenContentExceeds5000Characters()
    {
        var projectId = 1L;
        var comment = new ProjectComment(projectId, "Original content");
        var longContent = new string('A', 5001);

        Assert.Throws<ArgumentException>(() => comment.UpdateContent(longContent));
    }

    [Fact]
    public void ProjectComment_UpdateContent_ShouldAcceptContentWith5000Characters()
    {
        var projectId = 1L;
        var comment = new ProjectComment(projectId, "Original content");
        var maxContent = new string('A', 5000);

        comment.UpdateContent(maxContent);

        Assert.Equal(maxContent, comment.Content);
    }

    // --- TicketComment Tests ---

    [Fact]
    public void TicketComment_Constructor_ShouldSetProperties_WhenValidArgumentsAreProvided()
    {
        var ticketId = 1L;
        var content = "This is a ticket comment.";

        var comment = new TicketComment(ticketId, content);

        Assert.Equal(ticketId, comment.TicketId);
        Assert.Equal(content, comment.Content);
    }

    [Fact]
    public void TicketComment_Constructor_ShouldThrowException_WhenContentIsNull()
    {
        var ticketId = 1L;

        Assert.Throws<ArgumentException>(() => new TicketComment(ticketId, null!));
    }

    [Fact]
    public void TicketComment_Constructor_ShouldThrowException_WhenContentIsEmpty()
    {
        var ticketId = 1L;

        Assert.Throws<ArgumentException>(() => new TicketComment(ticketId, string.Empty));
    }

    [Fact]
    public void TicketComment_Constructor_ShouldThrowException_WhenContentIsWhitespace()
    {
        var ticketId = 1L;

        Assert.Throws<ArgumentException>(() => new TicketComment(ticketId, "   "));
    }

    [Fact]
    public void TicketComment_Constructor_ShouldThrowException_WhenContentExceeds5000Characters()
    {
        var ticketId = 1L;
        var longContent = new string('A', 5001);

        Assert.Throws<ArgumentException>(() => new TicketComment(ticketId, longContent));
    }

    [Fact]
    public void TicketComment_Constructor_ShouldAcceptContentWith5000Characters()
    {
        var ticketId = 1L;
        var maxContent = new string('A', 5000);

        var comment = new TicketComment(ticketId, maxContent);

        Assert.Equal(maxContent, comment.Content);
    }

    [Fact]
    public void TicketComment_UpdateContent_ShouldUpdateContent()
    {
        var ticketId = 1L;
        var comment = new TicketComment(ticketId, "Original content");

        comment.UpdateContent("Updated content");

        Assert.Equal("Updated content", comment.Content);
    }

    [Fact]
    public void TicketComment_UpdateContent_ShouldThrowException_WhenContentIsNull()
    {
        var ticketId = 1L;
        var comment = new TicketComment(ticketId, "Original content");

        Assert.Throws<ArgumentException>(() => comment.UpdateContent(null!));
    }

    [Fact]
    public void TicketComment_UpdateContent_ShouldThrowException_WhenContentIsEmpty()
    {
        var ticketId = 1L;
        var comment = new TicketComment(ticketId, "Original content");

        Assert.Throws<ArgumentException>(() => comment.UpdateContent(string.Empty));
    }

    [Fact]
    public void TicketComment_UpdateContent_ShouldThrowException_WhenContentIsWhitespace()
    {
        var ticketId = 1L;
        var comment = new TicketComment(ticketId, "Original content");

        Assert.Throws<ArgumentException>(() => comment.UpdateContent("   "));
    }

    [Fact]
    public void TicketComment_UpdateContent_ShouldThrowException_WhenContentExceeds5000Characters()
    {
        var ticketId = 1L;
        var comment = new TicketComment(ticketId, "Original content");
        var longContent = new string('A', 5001);

        Assert.Throws<ArgumentException>(() => comment.UpdateContent(longContent));
    }

    [Fact]
    public void TicketComment_UpdateContent_ShouldAcceptContentWith5000Characters()
    {
        var ticketId = 1L;
        var comment = new TicketComment(ticketId, "Original content");
        var maxContent = new string('A', 5000);

        comment.UpdateContent(maxContent);

        Assert.Equal(maxContent, comment.Content);
    }

    // --- Polymorphism Tests ---

    [Fact]
    public void Comment_ShouldBePolymorphic_ProjectCommentIsComment()
    {
        var comment = new ProjectComment(1L, "Test content");

        Assert.IsAssignableFrom<Comment>(comment);
    }

    [Fact]
    public void Comment_ShouldBePolymorphic_TicketCommentIsComment()
    {
        var comment = new TicketComment(1L, "Test content");

        Assert.IsAssignableFrom<Comment>(comment);
    }

    [Fact]
    public void Comment_ShouldInheritFromDomainEntity()
    {
        var projectComment = new ProjectComment(1L, "Test content");
        var ticketComment = new TicketComment(1L, "Test content");

        Assert.IsAssignableFrom<DomainEntity<long>>(projectComment);
        Assert.IsAssignableFrom<DomainEntity<long>>(ticketComment);
    }

    // --- Edge Cases ---

    [Fact]
    public void ProjectComment_Content_ShouldAcceptSingleCharacter()
    {
        var projectId = 1L;
        var content = "A";

        var comment = new ProjectComment(projectId, content);

        Assert.Equal(content, comment.Content);
    }

    [Fact]
    public void TicketComment_Content_ShouldAcceptSingleCharacter()
    {
        var ticketId = 1L;
        var content = "A";

        var comment = new TicketComment(ticketId, content);

        Assert.Equal(content, comment.Content);
    }

    [Fact]
    public void ProjectComment_Content_ShouldTrimLeadingAndTrailingWhitespace_NotApplied()
    {
        var projectId = 1L;
        var content = "  Content with spaces  ";

        var comment = new ProjectComment(projectId, content);

        // Content is stored as-is, not trimmed
        Assert.Equal(content, comment.Content);
    }

    [Fact]
    public void TicketComment_Content_ShouldTrimLeadingAndTrailingWhitespace_NotApplied()
    {
        var ticketId = 1L;
        var content = "  Content with spaces  ";

        var comment = new TicketComment(ticketId, content);

        // Content is stored as-is, not trimmed
        Assert.Equal(content, comment.Content);
    }

    [Fact]
    public void ProjectComment_Content_ShouldAcceptMultilineContent()
    {
        var projectId = 1L;
        var content = "Line 1\nLine 2\nLine 3";

        var comment = new ProjectComment(projectId, content);

        Assert.Equal(content, comment.Content);
    }

    [Fact]
    public void TicketComment_Content_ShouldAcceptMultilineContent()
    {
        var ticketId = 1L;
        var content = "Line 1\nLine 2\nLine 3";

        var comment = new TicketComment(ticketId, content);

        Assert.Equal(content, comment.Content);
    }

    [Fact]
    public void ProjectComment_Content_ShouldAcceptSpecialCharacters()
    {
        var projectId = 1L;
        var content = "Special chars: !@#$%^&*()_+-=[]{}|;':\",./<>?";

        var comment = new ProjectComment(projectId, content);

        Assert.Equal(content, comment.Content);
    }

    [Fact]
    public void TicketComment_Content_ShouldAcceptSpecialCharacters()
    {
        var ticketId = 1L;
        var content = "Special chars: !@#$%^&*()_+-=[]{}|;':\",./<>?";

        var comment = new TicketComment(ticketId, content);

        Assert.Equal(content, comment.Content);
    }

    [Fact]
    public void ProjectComment_Content_ShouldAcceptUnicodeCharacters()
    {
        var projectId = 1L;
        var content = "Unicode: 你好 🚀 Ñoño";

        var comment = new ProjectComment(projectId, content);

        Assert.Equal(content, comment.Content);
    }

    [Fact]
    public void TicketComment_Content_ShouldAcceptUnicodeCharacters()
    {
        var ticketId = 1L;
        var content = "Unicode: 你好 🚀 Ñoño";

        var comment = new TicketComment(ticketId, content);

        Assert.Equal(content, comment.Content);
    }

    [Fact]
    public void ProjectComment_UpdateContent_ShouldAllowMultipleUpdates()
    {
        var projectId = 1L;
        var comment = new ProjectComment(projectId, "First content");

        comment.UpdateContent("Second content");
        Assert.Equal("Second content", comment.Content);

        comment.UpdateContent("Third content");
        Assert.Equal("Third content", comment.Content);

        comment.UpdateContent("Fourth content");
        Assert.Equal("Fourth content", comment.Content);
    }

    [Fact]
    public void TicketComment_UpdateContent_ShouldAllowMultipleUpdates()
    {
        var ticketId = 1L;
        var comment = new TicketComment(ticketId, "First content");

        comment.UpdateContent("Second content");
        Assert.Equal("Second content", comment.Content);

        comment.UpdateContent("Third content");
        Assert.Equal("Third content", comment.Content);

        comment.UpdateContent("Fourth content");
        Assert.Equal("Fourth content", comment.Content);
    }
}
