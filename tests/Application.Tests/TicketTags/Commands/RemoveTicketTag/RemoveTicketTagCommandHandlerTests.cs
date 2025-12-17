using Lucy.Application.Common.Interfaces;
using Lucy.Application.TicketTags.Commands.RemoveTicketTag;
using Lucy.Application.TicketTags.Repositories;
using Lucy.Domain.Entities;
using Moq;

namespace Lucy.Application.Tests.TicketTags.Commands.RemoveTicketTag;

public class RemoveTicketTagCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITicketTagRepository> _ticketTagRepositoryMock;
    private readonly RemoveTicketTagCommandHandler _handler;

    public RemoveTicketTagCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _ticketTagRepositoryMock = new Mock<ITicketTagRepository>();

        _unitOfWorkMock.Setup(u => u.TicketTags).Returns(_ticketTagRepositoryMock.Object);

        _handler = new RemoveTicketTagCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldRemoveTicketTag_WhenLinkExists()
    {
        // Arrange
        var ticket = new Ticket(1, 1, "PROJ-1", 1, "Title", "Description") { Id = 10 };
        var tag = new Tag(1, "BUG", "Bug", "Bugs", Domain.Enums.Color.Red) { Id = 5 };
        var ticketTag = new TicketTag(ticket, tag) { Id = 99 };

        _ticketTagRepositoryMock
            .Setup(r => r.GetByTicketAndTagAsync(10, 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticketTag);

        var command = new RemoveTicketTagCommand(5, 10);

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        _ticketTagRepositoryMock.Verify(r => r.Remove(ticketTag), Times.Once);
        _unitOfWorkMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrow_WhenLinkNotFound()
    {
        // Arrange
        _ticketTagRepositoryMock
            .Setup(r => r.GetByTicketAndTagAsync(10, 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TicketTag?)null);

        var command = new RemoveTicketTagCommand(5, 10);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _handler.HandleAsync(command, CancellationToken.None));
    }
}
