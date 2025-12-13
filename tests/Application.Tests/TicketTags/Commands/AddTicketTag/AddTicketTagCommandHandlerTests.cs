using Lucy.Application.Interfaces;
using Lucy.Application.TicketTags.Commands.AddTicketTag;
using Lucy.Application.TicketTags.Repositories;
using Lucy.Application.Tags.Repositories;
using Lucy.Application.Tickets.Repositories;
using Lucy.Domain.Entities;
using Moq;

namespace Lucy.Application.Tests.TicketTags.Commands.AddTicketTag;

public class AddTicketTagCommandHandlerTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ITicketTagRepository> _ticketTagRepositoryMock;
    private readonly Mock<ITagRepository> _tagRepositoryMock;
    private readonly Mock<ITicketRepository> _ticketRepositoryMock;
    private readonly AddTicketTagCommandHandler _handler;

    public AddTicketTagCommandHandlerTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _ticketTagRepositoryMock = new Mock<ITicketTagRepository>();
        _tagRepositoryMock = new Mock<ITagRepository>();
        _ticketRepositoryMock = new Mock<ITicketRepository>();

        _unitOfWorkMock.Setup(u => u.TicketTags).Returns(_ticketTagRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Tags).Returns(_tagRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Tickets).Returns(_ticketRepositoryMock.Object);

        _handler = new AddTicketTagCommandHandler(_unitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldAddTicketTag_WhenEntitiesExist()
    {
        // Arrange
        var ticket = new Ticket(1, 1, "PROJ-1", 1, "Title", "Description") { Id = 10 };
        var tag = new Tag(1, "BUG", "Bug", "Bugs", Domain.Enums.Color.Red) { Id = 5 };

        _ticketRepositoryMock
            .Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);
        _tagRepositoryMock
            .Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tag);

        var command = new AddTicketTagCommand(5, 10);

        // Act
        await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        _ticketTagRepositoryMock.Verify(
            r => r.AddAsync(
                It.Is<TicketTag>(tt => tt.TagId == 5 && tt.TicketId == 10),
                It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
