using Lucy.Application.Projects.Repositories;
using Lucy.Application.Statuses.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Lucy.Application.Tickets.Commands.CreateTicket;
using Lucy.Domain.Entities;
using Lucy.Domain.Enums;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Tickets.Commands.CreateTicket;

public class CreateTicketCommandValidatorTests : ApplicationTestBase
{
    private readonly Mock<IProjectReadOnlyRepository> _projectRepositoryMock;
    private readonly Mock<IStatusReadOnlyRepository> _statusRepositoryMock;
    private readonly CreateTicketCommandValidator _validator;

    public CreateTicketCommandValidatorTests()
    {
        _projectRepositoryMock = SetupReadOnlyRepository(u => u.Projects);
        _statusRepositoryMock = SetupReadOnlyRepository(u => u.Statuses);
        _validator = new CreateTicketCommandValidator(ReadOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task ValidateAsync_ShouldValidate()
    {
        // Arrange
        var status = new Status(1, "TODO", 1, "To Do", "Tasks to be done", Color.Gray);

        _projectRepositoryMock
            .Setup(u => u.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _statusRepositoryMock
            .Setup(u => u.ExistsByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _statusRepositoryMock
            .Setup(u => u.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(status);

        var command = new CreateTicketCommand(1, 2, "Test Ticket", "This is a test ticket");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_ShouldInvalidate_WhenProjectDoesNotExist()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(u => u.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new CreateTicketCommand(1, 2, "Test Ticket", "This is a test ticket");

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ProjectId");
    }

    [Fact]
    public async Task ValidateAsync_ShouldInvalidate_WhenStatusDoesNotExist()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(u => u.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _statusRepositoryMock
            .Setup(u => u.ExistsByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new CreateTicketCommand(1, 2, "Test Ticket", "This is a test ticket");

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
        var status = new Status(2, "TODO", 1, "To Do", "Tasks to be done", Color.Gray);

        _projectRepositoryMock
            .Setup(u => u.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _statusRepositoryMock
            .Setup(u => u.ExistsByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _statusRepositoryMock
            .Setup(u => u.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(status);

        var command = new CreateTicketCommand(1, 2, "Test Ticket", "This is a test ticket");

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
        var status = new Status(1, "TODO", 1, "To Do", "Tasks to be done", Color.Gray);

        _projectRepositoryMock
            .Setup(u => u.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _statusRepositoryMock
            .Setup(u => u.ExistsByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _statusRepositoryMock
            .Setup(u => u.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(status);

        var command = new CreateTicketCommand(1, 2, "", "This is a test ticket");

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
        var status = new Status(1, "TODO", 1, "To Do", "Tasks to be done", Color.Gray);

        _projectRepositoryMock
            .Setup(u => u.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _statusRepositoryMock
            .Setup(u => u.ExistsByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _statusRepositoryMock
            .Setup(u => u.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(status);

        var command = new CreateTicketCommand(1, 2, new string('A', 201), "This is a test ticket");

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
        var status = new Status(1, "TODO", 1, "To Do", "Tasks to be done", Color.Gray);

        _projectRepositoryMock
            .Setup(u => u.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _statusRepositoryMock
            .Setup(u => u.ExistsByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _statusRepositoryMock
            .Setup(u => u.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(status);

        var command = new CreateTicketCommand(1, 2, "Test Ticket", new string('A', 5001));

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Description");
    }
}
