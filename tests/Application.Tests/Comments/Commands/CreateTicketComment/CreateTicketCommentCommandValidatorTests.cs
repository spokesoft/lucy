using Lucy.Application.Comments.Commands.CreateTicketComment;
using Lucy.Application.Tests.Infrastructure;
using Lucy.Application.Tickets.Repositories;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Comments.Commands.CreateTicketComment;

public class CreateTicketCommentCommandValidatorTests : ApplicationTestBase
{
    private readonly Mock<ITicketReadOnlyRepository> _ticketRepositoryMock;
    private readonly CreateTicketCommentCommandValidator _validator;

    public CreateTicketCommentCommandValidatorTests()
    {
        _ticketRepositoryMock = SetupReadOnlyRepository(u => u.Tickets);
        _validator = new CreateTicketCommentCommandValidator(ReadOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task ValidateAsync_ShouldValidate()
    {
        // Arrange
        _ticketRepositoryMock
            .Setup(u => u.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new CreateTicketCommentCommand(1, "Valid ticket comment");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_ShouldInvalidate_WhenTicketDoesNotExist()
    {
        // Arrange
        _ticketRepositoryMock
            .Setup(u => u.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new CreateTicketCommentCommand(1, "Valid ticket comment");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "TicketId");
    }

    [Fact]
    public async Task ValidateAsync_ShouldInvalidate_WhenContentIsEmpty()
    {
        // Arrange
        _ticketRepositoryMock
            .Setup(u => u.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new CreateTicketCommentCommand(1, string.Empty);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Content");
    }
}
