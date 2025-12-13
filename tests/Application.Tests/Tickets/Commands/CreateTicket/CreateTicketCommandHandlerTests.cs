using Lucy.Application.Sequences.Repositories;
using Lucy.Application.Statuses.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Lucy.Application.Tickets.Commands.CreateTicket;
using Lucy.Application.Tickets.Repositories;
using Lucy.Domain.Entities;
using Lucy.Domain.Enums;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Tickets.Commands.CreateTicket;

public class CreateTicketCommandHandlerTests : ApplicationTestBase
{
    private readonly Mock<ITicketRepository> _ticketRepositoryMock;
    private readonly Mock<IStatusRepository> _statusRepositoryMock;
    private readonly Mock<ISequenceRepository> _sequenceRepositoryMock;
    private readonly CreateTicketCommandHandler _handler;

    public CreateTicketCommandHandlerTests()
    {
        _ticketRepositoryMock = SetupRepository(u => u.Tickets);
        _statusRepositoryMock = SetupRepository(u => u.Statuses);
        _sequenceRepositoryMock = SetupRepository(u => u.Sequences);
        SetupRepository(u => u.Projects); // Needed for project check
        _handler = new CreateTicketCommandHandler(UnitOfWorkMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldCreateTicket_WhenValidCommandIsGiven()
    {
        // Arrange
        var project = new Project("TEST", "Test Project", "Test Description");
        var status = new Status(1, "TODO", 1, "To Do", "Tasks to be done", Color.Gray);
        var sequence = new Sequence(SequenceType.Ticket, 1, 0, "TEST-{0}");

        UnitOfWorkMock.Setup(u => u.Projects.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        _statusRepositoryMock
            .Setup(repo => repo.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(status);

        _sequenceRepositoryMock
            .Setup(repo => repo.GetByTypeAsync(1, SequenceType.Ticket, It.IsAny<CancellationToken>()))
            .ReturnsAsync(sequence);

        _ticketRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<Ticket>(), It.IsAny<CancellationToken>()))
            .Callback<Ticket, CancellationToken>((ticket, _) => ticket.Id = 1)
            .Returns(Task.CompletedTask);

        var command = new CreateTicketCommand(1, 2, "Test Ticket", "This is a test ticket");

        // Act
        var result = await _handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.Equal(1, result);

        _ticketRepositoryMock.Verify(repo => repo.AddAsync(
            It.Is<Ticket>(t =>
                t.ProjectId == 1 &&
                t.StatusId == 2 &&
                t.Title == "Test Ticket" &&
                t.Description == "This is a test ticket"
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        _sequenceRepositoryMock.Verify(
            repo => repo.Update(sequence), Times.Once);

        UnitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowException_WhenProjectDoesNotExist()
    {
        // Arrange
        UnitOfWorkMock.Setup(u => u.Projects.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project)null!);

        var command = new CreateTicketCommand(1, 2, "Test Ticket", "This is a test ticket");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.HandleAsync(command, CancellationToken.None));
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowException_WhenStatusDoesNotExist()
    {
        // Arrange
        var project = new Project("TEST", "Test Project", "Test Description");

        UnitOfWorkMock.Setup(u => u.Projects.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        _statusRepositoryMock
            .Setup(repo => repo.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Status)null!);

        var command = new CreateTicketCommand(1, 2, "Test Ticket", "This is a test ticket");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.HandleAsync(command, CancellationToken.None));
    }

    [Fact]
    public async Task HandleAsync_ShouldThrowException_WhenSequenceNotFound()
    {
        // Arrange
        var project = new Project("TEST", "Test Project", "Test Description");
        var status = new Status(1, "TODO", 1, "To Do", "Tasks to be done", Color.Gray);

        UnitOfWorkMock.Setup(u => u.Projects.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        _statusRepositoryMock
            .Setup(repo => repo.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(status);

        _sequenceRepositoryMock
            .Setup(repo => repo.GetByTypeAsync(1, SequenceType.Ticket, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Sequence)null!);

        var command = new CreateTicketCommand(1, 2, "Test Ticket", "This is a test ticket");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => _handler.HandleAsync(command, CancellationToken.None));
    }
}
