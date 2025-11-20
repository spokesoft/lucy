using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Repositories;
using Lucy.Application.Statuses.Commands.CreateStatus;
using Lucy.Application.Statuses.Commands.DeleteStatus;
using Lucy.Application.Statuses.Commands.UpdateStatus;
using Lucy.Application.Statuses.Repositories;
using Lucy.Application.Tickets.Repositories;
using Lucy.Domain.Entities;
using Lucy.Domain.Enums;
using Moq;

namespace Lucy.Application.Tests.Statuses;

public class StatusCommandTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IReadOnlyUnitOfWork> _readOnlyUnitOfWorkMock;
    private readonly Mock<IStatusRepository> _statusRepositoryMock;
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly Mock<ITicketRepository> _ticketRepositoryMock;

    public StatusCommandTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _readOnlyUnitOfWorkMock = new Mock<IReadOnlyUnitOfWork>();
        _statusRepositoryMock = new Mock<IStatusRepository>();
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _ticketRepositoryMock = new Mock<ITicketRepository>();

        _unitOfWorkMock.Setup(u => u.Statuses).Returns(_statusRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Projects).Returns(_projectRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Tickets).Returns(_ticketRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateStatusCommandHandler_ShouldCreateStatus_WhenValidCommandIsGiven()
    {
        // Arrange
        var project = new Project("TEST", "Test Project", "Test Description");

        _projectRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        _statusRepositoryMock
            .Setup(repo => repo.GetByProjectIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Status>());

        _statusRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<Status>(), It.IsAny<CancellationToken>()))
            .Callback<Status, CancellationToken>((status, _) => status.Id = 1)
            .Returns(Task.CompletedTask);

        var handler = new CreateStatusCommandHandler(_unitOfWorkMock.Object);
        var command = new CreateStatusCommand(1, "TODO", null, "To Do", "Tasks to be done", StatusColor.Gray);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result > 0);

        _statusRepositoryMock.Verify(repo => repo.AddAsync(
            It.Is<Status>(s =>
                s.ProjectId == 1 &&
                s.Key == "TODO" &&
                s.Name == "To Do" &&
                s.Description == "Tasks to be done" &&
                s.Color == StatusColor.Gray
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task CreateStatusCommandHandler_ShouldCreateStatusWithSpecifiedOrder_WhenOrderIsProvided()
    {
        // Arrange
        var project = new Project("TEST", "Test Project", "Test Description");

        _projectRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        _statusRepositoryMock
            .Setup(repo => repo.GetByProjectIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Status>());

        _statusRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<Status>(), It.IsAny<CancellationToken>()))
            .Callback<Status, CancellationToken>((status, _) => status.Id = 1)
            .Returns(Task.CompletedTask);

        var handler = new CreateStatusCommandHandler(_unitOfWorkMock.Object);
        var command = new CreateStatusCommand(1, "TODO", 1, "To Do", "Tasks to be done", StatusColor.Gray);

        // Act
        var result = await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result > 0);
        _statusRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Status>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CreateStatusCommandHandler_ShouldThrowException_WhenProjectDoesNotExist()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Project)null!);

        var handler = new CreateStatusCommandHandler(_unitOfWorkMock.Object);
        var command = new CreateStatusCommand(1, "TODO", null, "To Do", "Tasks to be done", StatusColor.Gray);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.HandleAsync(command, CancellationToken.None));
    }

    [Fact]
    public async Task CreateStatusCommandHandler_ShouldThrowException_WhenKeyIsEmpty()
    {
        // Arrange
        var project = new Project("TEST", "Test Project", "Test Description");

        _projectRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(project);

        _statusRepositoryMock
            .Setup(repo => repo.GetByProjectIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Status>());

        var handler = new CreateStatusCommandHandler(_unitOfWorkMock.Object);
        var command = new CreateStatusCommand(1, "", null, "To Do", "Tasks to be done", StatusColor.Gray);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(
            () => handler.HandleAsync(command, CancellationToken.None));
    }

    [Fact]
    public async Task DeleteStatusCommandHandler_ShouldDeleteStatus_WhenStatusExists()
    {
        // Arrange
        var status = new Status(1, "TODO", 1, "To Do", "Tasks to be done", StatusColor.Gray);
        var done = new Status(2, "DONE", 2, "Done", "Completed tasks", StatusColor.Green);

        _statusRepositoryMock.Setup(
            repo => repo.GetByIdAsync(1, CancellationToken.None)).ReturnsAsync(status);

        _statusRepositoryMock.Setup(
            repo => repo.GetByProjectIdAsync(status.ProjectId, CancellationToken.None))
            .ReturnsAsync([status, done]);

        _statusRepositoryMock.Setup(
            repo => repo.GetByKeyAsync(status.ProjectId, status.Key, CancellationToken.None))
            .ReturnsAsync(status);

        _ticketRepositoryMock.Setup(
            repo => repo.GetByStatusIdAsync(1, CancellationToken.None)).ReturnsAsync([]);

        var handler = new DeleteStatusCommandHandler(_unitOfWorkMock.Object);
        var command = new DeleteStatusCommand(1);

        // Act
        await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        _statusRepositoryMock.Verify(repo => repo.Remove(status), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task DeleteStatusCommandHandler_ShouldThrowException_WhenStatusDoesNotExist()
    {
        // Arrange
        _statusRepositoryMock.Setup(
            repo => repo.GetByIdAsync(1, CancellationToken.None)).ReturnsAsync((Status)null!);

        var handler = new DeleteStatusCommandHandler(_unitOfWorkMock.Object);
        var command = new DeleteStatusCommand(1);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.HandleAsync(command, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateStatusCommandHandler_ShouldUpdateStatus_WhenValidCommandIsGiven()
    {
        // Arrange
        var status = new Status(1, "TODO", 1, "To Do", "Old Description", StatusColor.Gray);

        _statusRepositoryMock.Setup(
            repo => repo.GetByIdAsync(1, CancellationToken.None)).ReturnsAsync(status);

        var handler = new UpdateStatusCommandHandler(_unitOfWorkMock.Object);
        var command = new UpdateStatusCommand(1, null, null, "New Name", "New Description", StatusColor.Blue);

        // Act
        await handler.HandleAsync(command, CancellationToken.None);

        // Assert
        Assert.Equal("New Name", status.Name);
        Assert.Equal("New Description", status.Description);
        Assert.Equal(StatusColor.Blue, status.Color);

        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task UpdateStatusCommandHandler_ShouldThrowException_WhenStatusDoesNotExist()
    {
        // Arrange
        _statusRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, CancellationToken.None))
            .ReturnsAsync((Status)null!);

        var handler = new UpdateStatusCommandHandler(_unitOfWorkMock.Object);
        var command = new UpdateStatusCommand(1, null, null, "New Name", "New Description", StatusColor.Blue);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler
            .HandleAsync(command, CancellationToken.None));
    }

    [Fact]
    public async Task CreateStatusCommandValidator_ShouldValidate()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Statuses.GetByProjectIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Status>());

        var validator = new CreateStatusCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new CreateStatusCommand(1, "TODO", null, "To Do", "Tasks to be done", StatusColor.Gray);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task CreateStatusCommandValidator_ShouldInvalidate_WhenProjectDoesNotExist()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var validator = new CreateStatusCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new CreateStatusCommand(1, "TODO", null, "To Do", "Tasks to be done", StatusColor.Gray);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ProjectId");
    }

    [Fact]
    public async Task CreateStatusCommandValidator_ShouldInvalidate_WhenKeyIsInvalid()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Statuses.GetByProjectIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Status>());

        var validator = new CreateStatusCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new CreateStatusCommand(1, "", null, "To Do", "Tasks to be done", StatusColor.Gray);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Key");
    }

    [Fact]
    public async Task CreateStatusCommandValidator_ShouldInvalidate_WhenNameIsInvalid()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Statuses.GetByProjectIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Status>());

        var validator = new CreateStatusCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new CreateStatusCommand(1, "TODO", null, new string('A', 51), "Tasks to be done", StatusColor.Gray);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task CreateStatusCommandValidator_ShouldInvalidate_WhenDescriptionIsInvalid()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Statuses.GetByProjectIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Status>());

        var validator = new CreateStatusCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new CreateStatusCommand(1, "TODO", null, "To Do", new string('A', 101), StatusColor.Gray);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Description");
    }

    [Fact]
    public async Task CreateStatusCommandValidator_ShouldInvalidate_WhenKeyIsNotUnique()
    {
        // Arrange
        var existingStatus = new Status(1, "TODO", 1, "Existing", "Existing Status", StatusColor.Gray);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Statuses.GetByProjectIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Status> { existingStatus });

        var validator = new CreateStatusCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new CreateStatusCommand(1, "TODO", null, "To Do", "Tasks to be done", StatusColor.Gray);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Key");
    }

    [Fact]
    public async Task CreateStatusCommandValidator_ShouldInvalidate_WhenOrderIsInvalid()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Statuses.GetByProjectIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Status>());

        var validator = new CreateStatusCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new CreateStatusCommand(1, "TODO", -1, "To Do", "Tasks to be done", StatusColor.Gray);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Order");
    }

    [Fact]
    public async Task UpdateStatusCommandValidator_ShouldValidate()
    {
        // Arrange
        var status = new Status(1, "TODO", 1, "To Do", "Tasks to be done", StatusColor.Gray);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Statuses.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(status);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Statuses.GetByProjectIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Status> { status });

        var validator = new UpdateStatusCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new UpdateStatusCommand(1, "DONE", null, "Done", "Completed tasks", StatusColor.Green);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task UpdateStatusCommandValidator_ShouldInvalidate_WhenStatusDoesNotExist()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Statuses.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Status)null!);

        var validator = new UpdateStatusCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new UpdateStatusCommand(1, "DONE", null, "Done", "Completed tasks", StatusColor.Green);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Id");
    }

    [Fact]
    public async Task UpdateStatusCommandValidator_ShouldInvalidate_WhenKeyIsInvalid()
    {
        // Arrange
        var status = new Status(1, "TODO", 1, "To Do", "Tasks to be done", StatusColor.Gray);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Statuses.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(status);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Statuses.GetByProjectIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Status> { status });

        var validator = new UpdateStatusCommandValidator(_readOnlyUnitOfWorkMock.Object);

        var command1 = new UpdateStatusCommand(1, string.Empty, null, "Done", "Completed", StatusColor.Green);
        var command2 = new UpdateStatusCommand(1, " ", null, "Done", "Completed", StatusColor.Green);
        var command3 = new UpdateStatusCommand(1, "1ABC", null, "Done", "Completed", StatusColor.Green);
        var command4 = new UpdateStatusCommand(1, "KEYISTOOLONGHERE", null, "Done", "Completed", StatusColor.Green);

        // Act
        var result1 = await validator.ValidateAsync(command1);
        var result2 = await validator.ValidateAsync(command2);
        var result3 = await validator.ValidateAsync(command3);
        var result4 = await validator.ValidateAsync(command4);

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
    public async Task UpdateStatusCommandValidator_ShouldInvalidate_WhenKeyIsNotUnique()
    {
        // Arrange
        var status1 = new Status(1, "TODO", 1, "To Do", "Tasks to be done", StatusColor.Gray);
        var status2 = new Status(1, "DONE", 2, "Done", "Completed", StatusColor.Green);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Statuses.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(status1);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Statuses.GetByProjectIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Status> { status1, status2 });

        var validator = new UpdateStatusCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new UpdateStatusCommand(1, "DONE", null, "To Do Updated", "Updated", StatusColor.Gray);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Key");
    }

    [Fact]
    public async Task UpdateStatusCommandValidator_ShouldInvalidate_WhenNameIsInvalid()
    {
        // Arrange
        var status = new Status(1, "TODO", 1, "To Do", "Tasks to be done", StatusColor.Gray);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Statuses.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(status);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Statuses.GetByProjectIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Status> { status });

        var validator = new UpdateStatusCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new UpdateStatusCommand(1, "DONE", null, new string('A', 51), "Completed", StatusColor.Green);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task UpdateStatusCommandValidator_ShouldInvalidate_WhenDescriptionIsInvalid()
    {
        // Arrange
        var status = new Status(1, "TODO", 1, "To Do", "Tasks to be done", StatusColor.Gray);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Statuses.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(status);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Statuses.GetByProjectIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Status> { status });

        var validator = new UpdateStatusCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new UpdateStatusCommand(1, "DONE", null, "Done", new string('A', 101), StatusColor.Green);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Description");
    }

    [Fact]
    public async Task DeleteStatusCommandValidator_ShouldValidate()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Statuses.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var validator = new DeleteStatusCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new DeleteStatusCommand(1);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task DeleteStatusCommandValidator_ShouldInvalidate_WhenStatusDoesNotExist()
    {
        // Arrange
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Statuses.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var validator = new DeleteStatusCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new DeleteStatusCommand(1);

        // Act
        var result = await validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Id");
    }
}
