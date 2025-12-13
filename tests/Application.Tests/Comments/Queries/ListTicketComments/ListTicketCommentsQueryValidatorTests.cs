using Lucy.Application.Comments.Queries.ListTicketComments;
using Lucy.Application.Tests.Infrastructure;
using Lucy.Application.Tickets.Repositories;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Comments.Queries.ListTicketComments;

public class ListTicketCommentsQueryValidatorTests : ApplicationTestBase
{
    private readonly Mock<ITicketReadOnlyRepository> _ticketRepositoryMock;
    private readonly ListTicketCommentsQueryValidator _validator;

    public ListTicketCommentsQueryValidatorTests()
    {
        _ticketRepositoryMock = SetupReadOnlyRepository(u => u.Tickets);
        _validator = new ListTicketCommentsQueryValidator(ReadOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task ValidateAsync_ShouldValidate()
    {
        // Arrange
        _ticketRepositoryMock
            .Setup(u => u.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var query = new ListTicketCommentsQuery(1);

        // Act
        var result = await _validator.ValidateAsync(query);

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

        var query = new ListTicketCommentsQuery(1);

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "TicketId");
    }
}
