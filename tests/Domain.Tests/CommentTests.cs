using Lucy.Domain.Entities;

namespace Lucy.Domain.Tests;

/// <summary>
/// Tests for the Comment domain entity.
/// </summary>
public class CommentTests
{
    [Fact]
    public void ProjectComment_Constructor_ShouldSetProperties_WhenValidArgumentsAreProvided()
    {
        // Arrange
        var projectId = 1L;
        var content = "This is a project comment.";

        // Act
        var comment = new ProjectComment(projectId, content);

        // Assert
        Assert.Equal(projectId, comment.ProjectId);
        Assert.Equal(content, comment.Content);
    }

    [Fact]
    public void ProjectComment_Constructor_ShouldThrowException_WhenContentIsNull()
    {
        // Arrange
        var projectId = 1L;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new ProjectComment(projectId, null!));
    }

    [Fact]
    public void ProjectComment_Constructor_ShouldThrowException_WhenContentIsEmpty()
    {
        // Arrange
        var projectId = 1L;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new ProjectComment(projectId, string.Empty));
    }

    [Fact]
    public void ProjectComment_Constructor_ShouldThrowException_WhenContentIsWhitespace()
    {
        // Arrange
        var projectId = 1L;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new ProjectComment(projectId, "   "));
    }

    [Fact]
    public void ProjectComment_Constructor_ShouldThrowException_WhenContentExceeds5000Characters()
    {
        // Arrange
        var projectId = 1L;
        var longContent = new string('A', 5001);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new ProjectComment(projectId, longContent));
    }

    [Fact]
    public void ProjectComment_Constructor_ShouldAcceptContentWith5000Characters()
    {
        // Arrange
        var projectId = 1L;
        var maxContent = new string('A', 5000);

        // Act
        var comment = new ProjectComment(projectId, maxContent);

        // Assert
        Assert.Equal(maxContent, comment.Content);
    }

    [Fact]
    public void ProjectComment_UpdateContent_ShouldUpdateContent()
    {
        // Arrange
        var projectId = 1L;
        var comment = new ProjectComment(projectId, "Original content");

        // Act
        comment.UpdateContent("Updated content");

        // Assert
        Assert.Equal("Updated content", comment.Content);
    }

    [Fact]
    public void ProjectComment_UpdateContent_ShouldThrowException_WhenContentIsNull()
    {
        // Arrange
        var projectId = 1L;
        var comment = new ProjectComment(projectId, "Original content");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => comment.UpdateContent(null!));
    }

    [Fact]
    public void ProjectComment_UpdateContent_ShouldThrowException_WhenContentIsEmpty()
    {
        // Arrange
        var projectId = 1L;
        var comment = new ProjectComment(projectId, "Original content");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => comment.UpdateContent(string.Empty));
    }

    [Fact]
    public void ProjectComment_UpdateContent_ShouldThrowException_WhenContentIsWhitespace()
    {
        // Arrange
        var projectId = 1L;
        var comment = new ProjectComment(projectId, "Original content");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => comment.UpdateContent("   "));
    }

    [Fact]
    public void ProjectComment_UpdateContent_ShouldThrowException_WhenContentExceeds5000Characters()
    {
        // Arrange
        var projectId = 1L;
        var comment = new ProjectComment(projectId, "Original content");
        var longContent = new string('A', 5001);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => comment.UpdateContent(longContent));
    }

    [Fact]
    public void ProjectComment_UpdateContent_ShouldAcceptContentWith5000Characters()
    {
        // Arrange
        var projectId = 1L;
        var comment = new ProjectComment(projectId, "Original content");
        var maxContent = new string('A', 5000);

        // Act
        comment.UpdateContent(maxContent);

        // Assert
        Assert.Equal(maxContent, comment.Content);
    }

    // --- TicketComment Tests ---

    [Fact]
    public void TicketComment_Constructor_ShouldSetProperties_WhenValidArgumentsAreProvided()
    {
        // Arrange
        var ticketId = 1L;
        var content = "This is a ticket comment.";

        // Act
        var comment = new TicketComment(ticketId, content);

        // Assert
        Assert.Equal(ticketId, comment.TicketId);
        Assert.Equal(content, comment.Content);
    }

    [Fact]
    public void TicketComment_Constructor_ShouldThrowException_WhenContentIsNull()
    {
        // Arrange
        var ticketId = 1L;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new TicketComment(ticketId, null!));
    }

    [Fact]
    public void TicketComment_Constructor_ShouldThrowException_WhenContentIsEmpty()
    {
        // Arrange
        var ticketId = 1L;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new TicketComment(ticketId, string.Empty));
    }

    [Fact]
    public void TicketComment_Constructor_ShouldThrowException_WhenContentIsWhitespace()
    {
        // Arrange
        var ticketId = 1L;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new TicketComment(ticketId, "   "));
    }

    [Fact]
    public void TicketComment_Constructor_ShouldThrowException_WhenContentExceeds5000Characters()
    {
        // Arrange
        var ticketId = 1L;
        var longContent = new string('A', 5001);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new TicketComment(ticketId, longContent));
    }

    [Fact]
    public void TicketComment_Constructor_ShouldAcceptContentWith5000Characters()
    {
        // Arrange
        var ticketId = 1L;
        var maxContent = new string('A', 5000);

        // Act
        var comment = new TicketComment(ticketId, maxContent);

        // Assert
        Assert.Equal(maxContent, comment.Content);
    }

    [Fact]
    public void TicketComment_UpdateContent_ShouldUpdateContent()
    {
        // Arrange
        var ticketId = 1L;
        var comment = new TicketComment(ticketId, "Original content");

        // Act
        comment.UpdateContent("Updated content");

        // Assert
        Assert.Equal("Updated content", comment.Content);
    }

    [Fact]
    public void TicketComment_UpdateContent_ShouldThrowException_WhenContentIsNull()
    {
        // Arrange
        var ticketId = 1L;
        var comment = new TicketComment(ticketId, "Original content");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => comment.UpdateContent(null!));
    }

    [Fact]
    public void TicketComment_UpdateContent_ShouldThrowException_WhenContentIsEmpty()
    {
        // Arrange
        var ticketId = 1L;
        var comment = new TicketComment(ticketId, "Original content");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => comment.UpdateContent(string.Empty));
    }

    [Fact]
    public void TicketComment_UpdateContent_ShouldThrowException_WhenContentIsWhitespace()
    {
        // Arrange
        var ticketId = 1L;
        var comment = new TicketComment(ticketId, "Original content");

        // Act & Assert
        Assert.Throws<ArgumentException>(() => comment.UpdateContent("   "));
    }

    [Fact]
    public void TicketComment_UpdateContent_ShouldThrowException_WhenContentExceeds5000Characters()
    {
        // Arrange
        var ticketId = 1L;
        var comment = new TicketComment(ticketId, "Original content");
        var longContent = new string('A', 5001);

        // Act & Assert
        Assert.Throws<ArgumentException>(() => comment.UpdateContent(longContent));
    }

    [Fact]
    public void TicketComment_UpdateContent_ShouldAcceptContentWith5000Characters()
    {
        // Arrange
        var ticketId = 1L;
        var comment = new TicketComment(ticketId, "Original content");
        var maxContent = new string('A', 5000);

        // Act
        comment.UpdateContent(maxContent);

        // Assert
        Assert.Equal(maxContent, comment.Content);
    }

    // --- Polymorphism Tests ---

    [Fact]
    public void Comment_ShouldBePolymorphic_ProjectCommentIsComment()
    {
        // Arrange & Act
        var comment = new ProjectComment(1L, "Test content");

        // Assert
        Assert.IsAssignableFrom<Comment>(comment);
    }

    [Fact]
    public void Comment_ShouldBePolymorphic_TicketCommentIsComment()
    {
        // Arrange & Act
        var comment = new TicketComment(1L, "Test content");

        // Assert
        Assert.IsAssignableFrom<Comment>(comment);
    }

    [Fact]
    public void Comment_ShouldInheritFromDomainEntity()
    {
        // Arrange & Act
        var projectComment = new ProjectComment(1L, "Test content");
        var ticketComment = new TicketComment(1L, "Test content");

        // Assert
        Assert.IsAssignableFrom<DomainEntity<long>>(projectComment);
        Assert.IsAssignableFrom<DomainEntity<long>>(ticketComment);
    }

    [Fact]
    public void ProjectComment_Content_ShouldAcceptSingleCharacter()
    {
        // Arrange
        var projectId = 1L;
        var content = "A";

        // Act
        var comment = new ProjectComment(projectId, content);

        // Assert
        Assert.Equal(content, comment.Content);
    }

    [Fact]
    public void TicketComment_Content_ShouldAcceptSingleCharacter()
    {
        // Arrange
        var ticketId = 1L;
        var content = "A";

        // Act
        var comment = new TicketComment(ticketId, content);

        // Assert
        Assert.Equal(content, comment.Content);
    }

    [Fact]
    public void ProjectComment_Content_ShouldTrimLeadingAndTrailingWhitespace_NotApplied()
    {
        // Arrange
        var projectId = 1L;
        var content = "  Content with spaces  ";

        // Act
        var comment = new ProjectComment(projectId, content);

        // Assert
        // Content is stored as-is, not trimmed
        Assert.Equal(content, comment.Content);
    }

    [Fact]
    public void TicketComment_Content_ShouldTrimLeadingAndTrailingWhitespace_NotApplied()
    {
        // Arrange
        var ticketId = 1L;
        var content = "  Content with spaces  ";

        // Act
        var comment = new TicketComment(ticketId, content);

        // Assert
        // Content is stored as-is, not trimmed
        Assert.Equal(content, comment.Content);
    }

    [Fact]
    public void ProjectComment_Content_ShouldAcceptMultilineContent()
    {
        // Arrange
        var projectId = 1L;
        var content = "Line 1\nLine 2\nLine 3";

        // Act
        var comment = new ProjectComment(projectId, content);

        // Assert
        Assert.Equal(content, comment.Content);
    }

    [Fact]
    public void TicketComment_Content_ShouldAcceptMultilineContent()
    {
        // Arrange
        var ticketId = 1L;
        var content = "Line 1\nLine 2\nLine 3";

        // Act
        var comment = new TicketComment(ticketId, content);

        // Assert
        Assert.Equal(content, comment.Content);
    }

    [Fact]
    public void ProjectComment_Content_ShouldAcceptSpecialCharacters()
    {
        // Arrange
        var projectId = 1L;
        var content = "Special chars: !@#$%^&*()_+-=[]{}|;':\",./<>?";

        // Act
        var comment = new ProjectComment(projectId, content);

        // Assert
        Assert.Equal(content, comment.Content);
    }

    [Fact]
    public void TicketComment_Content_ShouldAcceptSpecialCharacters()
    {
        // Arrange
        var ticketId = 1L;
        var content = "Special chars: !@#$%^&*()_+-=[]{}|;':\",./<>?";

        // Act
        var comment = new TicketComment(ticketId, content);

        // Assert
        Assert.Equal(content, comment.Content);
    }

    [Fact]
    public void ProjectComment_Content_ShouldAcceptUnicodeCharacters()
    {
        // Arrange
        var projectId = 1L;
        var content = "Unicode: 你好 🚀 Ñoño";

        // Act
        var comment = new ProjectComment(projectId, content);

        // Assert
        Assert.Equal(content, comment.Content);
    }

    [Fact]
    public void TicketComment_Content_ShouldAcceptUnicodeCharacters()
    {
        // Arrange
        var ticketId = 1L;
        var content = "Unicode: 你好 🚀 Ñoño";

        // Act
        var comment = new TicketComment(ticketId, content);

        // Assert
        Assert.Equal(content, comment.Content);
    }

    [Fact]
    public void ProjectComment_UpdateContent_ShouldAllowMultipleUpdates()
    {
        // Arrange
        var projectId = 1L;
        var comment = new ProjectComment(projectId, "First content");

        // Act & Assert
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
        // Arrange
        var ticketId = 1L;
        var comment = new TicketComment(ticketId, "First content");

        // Act & Assert
        comment.UpdateContent("Second content");
        Assert.Equal("Second content", comment.Content);

        comment.UpdateContent("Third content");
        Assert.Equal("Third content", comment.Content);

        comment.UpdateContent("Fourth content");
        Assert.Equal("Fourth content", comment.Content);
    }
}
