using Lucy.Application.Statuses.Commands.UpdateStatus;
using Lucy.Application.Statuses.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Lucy.Domain.Entities;
using Lucy.Domain.Enums;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Statuses.Commands.UpdateStatus;

public class UpdateStatusCommandValidatorTests : ApplicationTestBase
{
    private readonly Mock<IStatusReadOnlyRepository> _statusRepositoryMock;
    private readonly UpdateStatusCommandValidator _validator;

    public UpdateStatusCommandValidatorTests()
    {
        _statusRepositoryMock = SetupReadOnlyRepository(u => u.Statuses);
        _validator = new UpdateStatusCommandValidator(ReadOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task ValidateAsync_ShouldValidate()
    {
        // Arrange
        var status = new Status(1, "TODO", 1, "To Do", "Tasks to be done", Color.Gray);

        _statusRepositoryMock
            .Setup(u => u.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(status);

        _statusRepositoryMock
            .Setup(u => u.GetByProjectIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Status> { status });

        var command = new UpdateStatusCommand(1, "DONE", null, "Done", "Completed tasks", Color.Green);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_ShouldInvalidate_WhenStatusDoesNotExist()
    {
        // Arrange
        _statusRepositoryMock
            .Setup(u => u.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Status)null!);

        var command = new UpdateStatusCommand(1, "DONE", null, "Done", "Completed tasks", Color.Green);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Id");
    }

    [Fact]
    public async Task ValidateAsync_ShouldInvalidate_WhenKeyIsInvalid()
    {
        // Arrange
        var status = new Status(1, "TODO", 1, "To Do", "Tasks to be done", Color.Gray);

        _statusRepositoryMock
            .Setup(u => u.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(status);

        _statusRepositoryMock
            .Setup(u => u.GetByProjectIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Status> { status });

        var command1 = new UpdateStatusCommand(1, string.Empty, null, "Done", "Completed", Color.Green);
        var command2 = new UpdateStatusCommand(1, " ", null, "Done", "Completed", Color.Green);
        var command3 = new UpdateStatusCommand(1, "1ABC", null, "Done", "Completed", Color.Green);
        var command4 = new UpdateStatusCommand(1, "KEYISTOOLONGHERE", null, "Done", "Completed", Color.Green);

        // Act
        var result1 = await _validator.ValidateAsync(command1);
        var result2 = await _validator.ValidateAsync(command2);
        var result3 = await _validator.ValidateAsync(command3);
        var result4 = await _validator.ValidateAsync(command4);

        // Assert
        Assert.False(result1.IsValid);
        Assert.Contains(result1.Errors, e => e.PropertyName == "Key");

        Assert.False(result2.IsValid);
        Assert.Contains(result2.Errors, e => e.PropertyName == "Key");

        Assert.False(result3.IsValid);
        Assert.Contains(result3.Errors, e => e.PropertyName == "Key");

        Assert.False(result4.IsValid);
        Assert.Contains(result4.Errors, e => e.PropertyName == "Key");
    }

    [Fact]
    public async Task ValidateAsync_ShouldInvalidate_WhenKeyIsNotUnique()
    {
        // Arrange
        var status1 = new Status(1, "TODO", 1, "To Do", "Tasks to be done", Color.Gray);
        var status2 = new Status(1, "DONE", 2, "Done", "Completed", Color.Green);

        _statusRepositoryMock
            .Setup(u => u.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(status1);

        _statusRepositoryMock
            .Setup(u => u.GetByProjectIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Status> { status1, status2 });

        var command = new UpdateStatusCommand(1, "DONE", null, "To Do Updated", "Updated", Color.Gray);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Key");
    }

    [Fact]
    public async Task ValidateAsync_ShouldInvalidate_WhenNameIsInvalid()
    {
        // Arrange
        var status = new Status(1, "TODO", 1, "To Do", "Tasks to be done", Color.Gray);

        _statusRepositoryMock
            .Setup(u => u.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(status);

        _statusRepositoryMock
            .Setup(u => u.GetByProjectIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Status> { status });

        var command = new UpdateStatusCommand(1, "DONE", null, new string('A', 51), "Completed", Color.Green);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task ValidateAsync_ShouldInvalidate_WhenDescriptionIsInvalid()
    {
        // Arrange
        var status = new Status(1, "TODO", 1, "To Do", "Tasks to be done", Color.Gray);

        _statusRepositoryMock
            .Setup(u => u.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(status);

        _statusRepositoryMock
            .Setup(u => u.GetByProjectIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Status> { status });

        var command = new UpdateStatusCommand(1, "DONE", null, "Done", new string('A', 101), Color.Green);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Description");
    }
}
