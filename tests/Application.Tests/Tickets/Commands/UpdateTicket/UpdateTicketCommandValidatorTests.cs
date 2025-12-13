using Lucy.Application.Statuses.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Lucy.Application.Tickets.Commands.UpdateTicket;
using Lucy.Application.Tickets.Repositories;
using Lucy.Domain.Entities;
using Lucy.Domain.Enums;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Tickets.Commands.UpdateTicket;

public class UpdateTicketCommandValidatorTests : ApplicationTestBase
{
    private readonly Mock<ITicketReadOnlyRepository> _ticketRepositoryMock;
    private readonly Mock<IStatusReadOnlyRepository> _statusRepositoryMock;
    private readonly UpdateTicketCommandValidator _validator;

    public UpdateTicketCommandValidatorTests()
    {
        _ticketRepositoryMock = SetupReadOnlyRepository(u => u.Tickets);
        _statusRepositoryMock = SetupReadOnlyRepository(u => u.Statuses);
        _validator = new UpdateTicketCommandValidator(ReadOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task ValidateAsync_ShouldValidate()
    {
        // Arrange
        var ticket = new Ticket(1, 2, "TEST-1", 1, "Test Ticket", "This is a test ticket");
        var status = new Status(1, "TODO", 1, "To Do", "Tasks to be done", Color.Gray);

        _ticketRepositoryMock
            .Setup(u => u.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        _statusRepositoryMock
            .Setup(u => u.ExistsByIdAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _statusRepositoryMock
            .Setup(u => u.GetByIdAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(status);

        var command = new UpdateTicketCommand(1, 3, "New Title", "New Description");

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
            .Setup(u => u.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Ticket)null!);

        var command = new UpdateTicketCommand(1, 3, "New Title", "New Description");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Id");
    }

    [Fact]
    public async Task ValidateAsync_ShouldInvalidate_WhenStatusDoesNotExist()
    {
        // Arrange
        var ticket = new Ticket(1, 2, "TEST-1", 1, "Test Ticket", "This is a test ticket");

        _ticketRepositoryMock
            .Setup(u => u.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        _statusRepositoryMock
            .Setup(u => u.GetByIdAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Status)null!);

        var command = new UpdateTicketCommand(1, 3, "New Title", "New Description");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "StatusId");
    }

    [Fact]
    public async Task ValidateAsync_ShouldInvalidate_WhenStatusNotInProject()
    {
        // Arrange
        var ticket = new Ticket(1, 2, "TEST-1", 1, "Test Ticket", "This is a test ticket");
        var status = new Status(2, "TODO", 1, "To Do", "Tasks to be done", Color.Gray);

        _ticketRepositoryMock
            .Setup(u => u.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        _statusRepositoryMock
            .Setup(u => u.GetByIdAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(status);

        var command = new UpdateTicketCommand(1, 3, "New Title", "New Description");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "StatusId");
    }

    [Fact]
    public async Task ValidateAsync_ShouldInvalidate_WhenTitleIsEmpty()
    {
        // Arrange
        var ticket = new Ticket(1, 2, "TEST-1", 1, "Test Ticket", "This is a test ticket");

        _ticketRepositoryMock
            .Setup(u => u.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        var command = new UpdateTicketCommand(1, null, "", null);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Title");
    }

    [Fact]
    public async Task ValidateAsync_ShouldInvalidate_WhenTitleTooLong()
    {
        // Arrange
        var ticket = new Ticket(1, 2, "TEST-1", 1, "Test Ticket", "This is a test ticket");

        _ticketRepositoryMock
            .Setup(u => u.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        var command = new UpdateTicketCommand(1, null, new string('A', 201), null);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Title");
    }

    [Fact]
    public async Task ValidateAsync_ShouldInvalidate_WhenDescriptionTooLong()
    {
        // Arrange
        var ticket = new Ticket(1, 2, "TEST-1", 1, "Test Ticket", "This is a test ticket");

        _ticketRepositoryMock
            .Setup(u => u.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        var command = new UpdateTicketCommand(1, null, null, new string('A', 5001));

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Description");
    }

    [Fact]
    public async Task ValidateAsync_ShouldInvalidate_WhenNoDataToUpdate()
    {
        // Arrange
        var ticket = new Ticket(1, 2, "TEST-1", 1, "Test Ticket", "This is a test ticket");

        _ticketRepositoryMock
            .Setup(u => u.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);

        var command = new UpdateTicketCommand(1, null, null, null);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Command");
    }
}
