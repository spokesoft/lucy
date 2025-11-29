using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Repositories;
using Lucy.Application.Sequences.Repositories;
using Lucy.Application.Statuses.Repositories;
using Lucy.Application.Tickets.Commands.CreateTicket;
using Lucy.Application.Tickets.Commands.DeleteTicket;
using Lucy.Application.Tickets.Commands.UpdateTicket;
using Lucy.Application.Tickets.Repositories;
using Lucy.Domain.Entities;
using Lucy.Domain.Enums;
using Moq;

namespace Lucy.Application.Tests.Tickets;

public class TicketCommandTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IReadOnlyUnitOfWork> _readOnlyUnitOfWorkMock;
    private readonly Mock<ITicketRepository> _ticketRepositoryMock;
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly Mock<IStatusRepository> _statusRepositoryMock;
    private readonly Mock<ISequenceRepository> _sequenceRepositoryMock;

    public TicketCommandTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _readOnlyUnitOfWorkMock = new Mock<IReadOnlyUnitOfWork>();
        _ticketRepositoryMock = new Mock<ITicketRepository>();
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _statusRepositoryMock = new Mock<IStatusRepository>();
        _sequenceRepositoryMock = new Mock<ISequenceRepository>();
        _unitOfWorkMock.Setup(u => u.Tickets).Returns(_ticketRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Projects).Returns(_projectRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Statuses).Returns(_statusRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Sequences).Returns(_sequenceRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateTicketCommandHandler_ShouldCreateTicket_WhenValidCommandIsGiven()
    {
        // Arrange
        var project = new Project("TEST", "Test Project", "Test Description");
        var status = new Status(1, "TODO", 1, "To Do", "Tasks to be done", Color.Gray);
        var sequence = new Sequence(SequenceType.Ticket, 1, 0, "TEST-{0}");

        _projectRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
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

        var handler = new CreateTicketCommandHandler(_unitOfWorkMock.Object);
        var command = new CreateTicketCommand(1, 2, "Test Ticket", "This is a test ticket");

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result > 0);

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

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task CreateTicketCommandHandler_ShouldThrowException_WhenProjectDoesNotExist()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project)null!);

        var handler = new CreateTicketCommandHandler(_unitOfWorkMock.Object);
        var command = new CreateTicketCommand(1, 2, "Test Ticket", "This is a test ticket");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.HandleAsync(command, CancellationToken.None));
    }

    [Fact]
    public async Task CreateTicketCommandHandler_ShouldThrowException_WhenStatusDoesNotExist()
    {
        // Arrange
        var project = new Project("TEST", "Test Project", "Test Description");

        _projectRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        _statusRepositoryMock
            .Setup(repo => repo.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Status)null!);

        var handler = new CreateTicketCommandHandler(_unitOfWorkMock.Object);
        var command = new CreateTicketCommand(1, 2, "Test Ticket", "This is a test ticket");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.HandleAsync(command, CancellationToken.None));
    }

    [Fact]
    public async Task CreateTicketCommandHandler_ShouldThrowException_WhenSequenceNotFound()
    {
        // Arrange
        var project = new Project("TEST", "Test Project", "Test Description");
        var status = new Status(1, "TODO", 1, "To Do", "Tasks to be done", Color.Gray);

        _projectRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        _statusRepositoryMock
            .Setup(repo => repo.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(status);

        _sequenceRepositoryMock
            .Setup(repo => repo.GetByTypeAsync(1, SequenceType.Ticket, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Sequence)null!);

        var handler = new CreateTicketCommandHandler(_unitOfWorkMock.Object);
        var command = new CreateTicketCommand(1, 2, "Test Ticket", "This is a test ticket");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.HandleAsync(command, CancellationToken.None));
    }

    [Fact]
    public async Task DeleteTicketCommandHandler_ShouldDeleteTicket_WhenTicketExists()
    {
        // Arrange
        var ticket = new Ticket(1, 2, "TEST-1", 1, "Test Ticket", "This is a test ticket");

        _ticketRepositoryMock.Setup(
            repo => repo.GetByIdAsync(1, CancellationToken.None)).ReturnsAsync(ticket);

        var handler = new DeleteTicketCommandHandler(_unitOfWorkMock.Object);
        var command = new DeleteTicketCommand(1);

        // Act
        await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        _ticketRepositoryMock.Verify(repo => repo.Remove(ticket), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task DeleteTicketCommandHandler_ShouldThrowException_WhenTicketDoesNotExist()
    {
        // Arrange
        _ticketRepositoryMock.Setup(
            repo => repo.GetByIdAsync(1, CancellationToken.None)).ReturnsAsync((Ticket)null!);

        var handler = new DeleteTicketCommandHandler(_unitOfWorkMock.Object);
        var command = new DeleteTicketCommand(1);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.HandleAsync(command, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateTicketCommandHandler_ShouldUpdateStatus_WhenStatusIdIsProvided()
    {
        // Arrange
        var ticket = new Ticket(1, 2, "TEST-1", 1, "Test Ticket", "This is a test ticket");

        _ticketRepositoryMock.Setup(
            repo => repo.GetByIdAsync(1, CancellationToken.None)).ReturnsAsync(ticket);

        var handler = new UpdateTicketCommandHandler(_unitOfWorkMock.Object);
        var command = new UpdateTicketCommand(1, 3, null, null);

        // Act
        await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.Equal(3L, ticket.StatusId);
        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task UpdateTicketCommandHandler_ShouldUpdateTitle_WhenTitleIsProvided()
    {
        // Arrange
        var ticket = new Ticket(1, 2, "TEST-1", 1, "Old Title", "This is a test ticket");

        _ticketRepositoryMock.Setup(
            repo => repo.GetByIdAsync(1, CancellationToken.None)).ReturnsAsync(ticket);

        var handler = new UpdateTicketCommandHandler(_unitOfWorkMock.Object);
        var command = new UpdateTicketCommand(1, null, "New Title", null);

        // Act
        await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.Equal("New Title", ticket.Title);
        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task UpdateTicketCommandHandler_ShouldUpdateDescription_WhenDescriptionIsProvided()
    {
        // Arrange
        var ticket = new Ticket(1, 2, "TEST-1", 1, "Test Ticket", "Old Description");

        _ticketRepositoryMock.Setup(
            repo => repo.GetByIdAsync(1, CancellationToken.None)).ReturnsAsync(ticket);

        var handler = new UpdateTicketCommandHandler(_unitOfWorkMock.Object);
        var command = new UpdateTicketCommand(1, null, null, "New Description");

        // Act
        await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.Equal("New Description", ticket.Description);
        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task UpdateTicketCommandHandler_ShouldUpdateAllProperties_WhenAllAreProvided()
    {
        // Arrange
        var ticket = new Ticket(1, 2, "TEST-1", 1, "Old Title", "Old Description");

        _ticketRepositoryMock.Setup(
            repo => repo.GetByIdAsync(1, CancellationToken.None)).ReturnsAsync(ticket);

        var handler = new UpdateTicketCommandHandler(_unitOfWorkMock.Object);
        var command = new UpdateTicketCommand(1, 3, "New Title", "New Description");

        // Act
        await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.Equal(3L, ticket.StatusId);
        Assert.Equal("New Title", ticket.Title);
        Assert.Equal("New Description", ticket.Description);
        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task UpdateTicketCommandHandler_ShouldThrowException_WhenTicketDoesNotExist()
    {
        // Arrange
        _ticketRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, CancellationToken.None))
            .ReturnsAsync((Ticket)null!);

        var handler = new UpdateTicketCommandHandler(_unitOfWorkMock.Object);
        var command = new UpdateTicketCommand(1, 3, "New Title", "New Description");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler
            .HandleAsync(command, CancellationToken.None));
    }

    [Fact]
    public async Task CreateTicketCommandValidator_ShouldValidate()
    {
        // Arrange
        var project = new Project("TEST", "Test Project", "Test Description");
        var status = new Status(1, "TODO", 1, "To Do", "Tasks to be done", Color.Gray);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Statuses.ExistsByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Statuses.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(status);

        var validator = new CreateTicketCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new CreateTicketCommand(1, 2, "Test Ticket", "This is a test ticket");

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task CreateTicketCommandValidator_ShouldInvalidate_WhenProjectDoesNotExist()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project)null!);

        var validator = new CreateTicketCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new CreateTicketCommand(1, 2, "Test Ticket", "This is a test ticket");

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ProjectId");
    }

    [Fact]
    public async Task CreateTicketCommandValidator_ShouldInvalidate_WhenStatusDoesNotExist()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Statuses.ExistsByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var validator = new CreateTicketCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new CreateTicketCommand(1, 2, "Test Ticket", "This is a test ticket");

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "StatusId");
    }

    [Fact]
    public async Task CreateTicketCommandValidator_ShouldInvalidate_WhenStatusNotInProject()
    {
        // Arrange
        var status = new Status(2, "TODO", 1, "To Do", "Tasks to be done", Color.Gray);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Statuses.ExistsByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Statuses.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(status);

        var validator = new CreateTicketCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new CreateTicketCommand(1, 2, "Test Ticket", "This is a test ticket");

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "StatusId");
    }

    [Fact]
    public async Task CreateTicketCommandValidator_ShouldInvalidate_WhenTitleIsEmpty()
    {
        // Arrange
        var status = new Status(1, "TODO", 1, "To Do", "Tasks to be done", Color.Gray);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Statuses.ExistsByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Statuses.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(status);

        var validator = new CreateTicketCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new CreateTicketCommand(1, 2, "", "This is a test ticket");

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Title");
    }

    [Fact]
    public async Task CreateTicketCommandValidator_ShouldInvalidate_WhenTitleTooLong()
    {
        // Arrange
        var status = new Status(1, "TODO", 1, "To Do", "Tasks to be done", Color.Gray);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Statuses.ExistsByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Statuses.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(status);

        var validator = new CreateTicketCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new CreateTicketCommand(1, 2, new string('A', 201), "This is a test ticket");

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Title");
    }

    [Fact]
    public async Task CreateTicketCommandValidator_ShouldInvalidate_WhenDescriptionTooLong()
    {
        // Arrange
        var status = new Status(1, "TODO", 1, "To Do", "Tasks to be done", Color.Gray);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Statuses.ExistsByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Statuses.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(status);

        var validator = new CreateTicketCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new CreateTicketCommand(1, 2, "Test Ticket", new string('A', 5001));

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Description");
    }

    [Fact]
    public async Task UpdateTicketCommandValidator_ShouldValidate()
    {
        // Arrange
        var ticket = new Ticket(1, 2, "TEST-1", 1, "Test Ticket", "This is a test ticket");
        var status = new Status(1, "TODO", 1, "To Do", "Tasks to be done", Color.Gray);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Tickets.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Statuses.ExistsByIdAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Statuses.GetByIdAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(status);

        var validator = new UpdateTicketCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new UpdateTicketCommand(1, 3, "New Title", "New Description");

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task UpdateTicketCommandValidator_ShouldInvalidate_WhenTicketDoesNotExist()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Tickets.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Ticket)null!);

        var validator = new UpdateTicketCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new UpdateTicketCommand(1, 3, "New Title", "New Description");

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Id");
    }

    [Fact]
    public async Task UpdateTicketCommandValidator_ShouldInvalidate_WhenStatusDoesNotExist()
    {
        // Arrange
        var ticket = new Ticket(1, 2, "TEST-1", 1, "Test Ticket", "This is a test ticket");

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Tickets.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Statuses.GetByIdAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Status)null!);

        var validator = new UpdateTicketCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new UpdateTicketCommand(1, 3, "New Title", "New Description");

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "StatusId");
    }

    [Fact]
    public async Task UpdateTicketCommandValidator_ShouldInvalidate_WhenStatusNotInProject()
    {
        // Arrange
        var ticket = new Ticket(1, 2, "TEST-1", 1, "Test Ticket", "This is a test ticket");
        var status = new Status(2, "TODO", 1, "To Do", "Tasks to be done", Color.Gray);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Tickets.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Statuses.GetByIdAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(status);

        var validator = new UpdateTicketCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new UpdateTicketCommand(1, 3, "New Title", "New Description");

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "StatusId");
    }

    [Fact]
    public async Task UpdateTicketCommandValidator_ShouldInvalidate_WhenTitleIsEmpty()
    {
        // Arrange
        var ticket = new Ticket(1, 2, "TEST-1", 1, "Test Ticket", "This is a test ticket");

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Tickets.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        var validator = new UpdateTicketCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new UpdateTicketCommand(1, null, "", null);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Title");
    }

    [Fact]
    public async Task UpdateTicketCommandValidator_ShouldInvalidate_WhenTitleTooLong()
    {
        // Arrange
        var ticket = new Ticket(1, 2, "TEST-1", 1, "Test Ticket", "This is a test ticket");

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Tickets.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        var validator = new UpdateTicketCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new UpdateTicketCommand(1, null, new string('A', 201), null);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Title");
    }

    [Fact]
    public async Task UpdateTicketCommandValidator_ShouldInvalidate_WhenDescriptionTooLong()
    {
        // Arrange
        var ticket = new Ticket(1, 2, "TEST-1", 1, "Test Ticket", "This is a test ticket");

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Tickets.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        var validator = new UpdateTicketCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new UpdateTicketCommand(1, null, null, new string('A', 5001));

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Description");
    }

    [Fact]
    public async Task UpdateTicketCommandValidator_ShouldInvalidate_WhenNoDataToUpdate()
    {
        // Arrange
        var ticket = new Ticket(1, 2, "TEST-1", 1, "Test Ticket", "This is a test ticket");

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Tickets.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        var validator = new UpdateTicketCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new UpdateTicketCommand(1, null, null, null);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Command");
    }

    [Fact]
    public async Task DeleteTicketCommandValidator_ShouldValidate()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Tickets.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var validator = new DeleteTicketCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new DeleteTicketCommand(1);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task DeleteTicketCommandValidator_ShouldInvalidate_WhenTicketDoesNotExist()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Tickets.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var validator = new DeleteTicketCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new DeleteTicketCommand(1);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Id");
    }
}
