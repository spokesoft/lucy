using Lucy.Application.Interfaces;
using Lucy.Application.Projects.Commands.CreateProject;
using Lucy.Application.Projects.Commands.DeleteProject;
using Lucy.Application.Projects.Commands.UpdateProject;
using Lucy.Application.Projects.Repositories;
using Lucy.Domain.Entities;
using Moq;

namespace Lucy.Application.Tests;

public class ProjectCommandTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IReadOnlyUnitOfWork> _readOnlyUnitOfWorkMock;
    private readonly Mock<IProjectRepository> _projectRepositoryMock;

    public ProjectCommandTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _readOnlyUnitOfWorkMock = new Mock<IReadOnlyUnitOfWork>();
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _unitOfWorkMock.Setup(u => u.Projects).Returns(_projectRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateProjectCommandHandler_ShouldCreateProject_WhenValidCommandIsGiven()
    {
        _projectRepositoryMock
            .Setup(repo => repo.AddAsync(It.IsAny<Project>(), It.IsAny<CancellationToken>()))
            .Callback<Project, CancellationToken>((project, _) => project.Id = 1)
            .Returns(Task.CompletedTask);

        var handler = new CreateProjectCommandHandler(_unitOfWorkMock.Object);
        var command = new CreateProjectCommand("TEST_KEY", "Test Name", "Test Description");
        var result = await handler.HandleAsync(command, CancellationToken.None);

        Assert.True(result > 0);

        _projectRepositoryMock.Verify(repo => repo.AddAsync(
            It.Is<Project>(p =>
                p.Key == "TEST_KEY" &&
                p.Name == "Test Name" &&
                p.Description == "Test Description"
            ),
            It.IsAny<CancellationToken>()
        ), Times.Once);

        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task CreateProjectCommandHandler_ShouldThrowException_WhenKeyIsEmpty()
    {
        var handler = new CreateProjectCommandHandler(_unitOfWorkMock.Object);
        var command = new CreateProjectCommand("", "Test Name", "Test Description");

        await Assert.ThrowsAsync<ArgumentException>(
            () => handler.HandleAsync(command, CancellationToken.None));
    }

    [Fact]
    public async Task DeleteProjectCommandHandler_ShouldDeleteProject_WhenProjectExists()
    {
        var project = new Project("TEST_KEY", "Test Name", "Test Description");

        _projectRepositoryMock.Setup(
            repo => repo.GetByIdAsync(1, CancellationToken.None)).ReturnsAsync(project);

        var handler = new DeleteProjectCommandHandler(_unitOfWorkMock.Object);
        var command = new DeleteProjectCommand(1);

        await handler.HandleAsync(command, CancellationToken.None);

        _projectRepositoryMock.Verify(repo => repo.Remove(project), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task DeleteProjectCommandHandler_ShouldThrowException_WhenProjectDoesNotExist()
    {
        _projectRepositoryMock.Setup(
            repo => repo.GetByIdAsync(1, CancellationToken.None)).ReturnsAsync((Project)null!);

        var handler = new DeleteProjectCommandHandler(_unitOfWorkMock.Object);
        var command = new DeleteProjectCommand(1);

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => handler.HandleAsync(command, CancellationToken.None));
    }

    [Fact]
    public async Task UpdateProjectCommandHandler_ShouldUpdateProject_WhenValidCommandIsGiven()
    {
        var project = new Project("OLD_KEY", "Old Name", "Old Description");

        _projectRepositoryMock.Setup(
            repo => repo.GetByIdAsync(1, CancellationToken.None)).ReturnsAsync(project);

        var handler = new UpdateProjectCommandHandler(_unitOfWorkMock.Object);
        var command = new UpdateProjectCommand(1, "NEW_KEY", "New Name", "New Description");

        await handler.HandleAsync(command, CancellationToken.None);

        Assert.Equal("NEW_KEY", project.Key);
        Assert.Equal("New Name", project.Name);
        Assert.Equal("New Description", project.Description);

        _unitOfWorkMock.Verify(
            u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task UpdateProjectCommandHandler_ShouldThrowException_WhenProjectDoesNotExist()
    {
        _projectRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, CancellationToken.None))
            .ReturnsAsync((Project)null!);

        var handler = new UpdateProjectCommandHandler(_unitOfWorkMock.Object);
        var command = new UpdateProjectCommand(1, "NEW_KEY", "New Name", "New Description");

        await Assert.ThrowsAsync<InvalidOperationException>(() => handler
            .HandleAsync(command, CancellationToken.None));
    }

    [Fact]
    public async Task CreateProjectCommandValidator_ShouldValidate()
    {
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.ExistsByKeyAsync("VALID-KEY", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var validator = new CreateProjectCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new CreateProjectCommand("VALID-KEY", "Valid Name", "Valid Description");

        var result = await validator.ValidateAsync(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task CreateProjectCommandValidator_ShouldInvalidate_WhenKeyIsInvalid()
    {
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.ExistsByKeyAsync("VALID-KEY", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var validator = new CreateProjectCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new CreateProjectCommand("", "Valid Name", "Valid Description");

        var result = await validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Key");
    }

    [Fact]
    public async Task CreateProjectCommandValidator_ShouldInvalidate_WhenNameIsInvalid()
    {
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.ExistsByKeyAsync("VALID-KEY", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var validator = new CreateProjectCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new CreateProjectCommand("VALID-KEY", new string('A', 101), "Valid Description");

        var result = await validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task CreateProjectCommandValidator_ShouldInvalidate_WhenDescriptionIsInvalid()
    {
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.ExistsByKeyAsync("VALID-KEY", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var validator = new CreateProjectCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new CreateProjectCommand("VALID-KEY", "Valid Name", new string('A', 501));

        var result = await validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Description");
    }

    [Fact]
    public async Task CreateProjectCommandValidator_ShouldInvalidate_WhenKeyIsNotUnique()
    {
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.ExistsByKeyAsync("DUPLICATE", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var validator = new CreateProjectCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new CreateProjectCommand("DUPLICATE", "Valid Name", "Valid Description");

        var result = await validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Key");
    }

    [Fact]
    public async Task UpdateProjectCommandValidator_ShouldValidate()
    {
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.ExistsByKeyAsync("VALID-KEY", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var validator = new UpdateProjectCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new UpdateProjectCommand(1, "VALID-KEY", "Valid Name", "Valid Description");

        var result = await validator.ValidateAsync(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task UpdateProjectCommandValidator_ShouldInvalidate_WhenProjectDoesNotExist()
    {
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var validator = new UpdateProjectCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new UpdateProjectCommand(1, "VALID-KEY", "Valid Name", "Valid Description");

        var result = await validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Id");
    }

    [Fact]
    public async Task UpdateProjectCommandValidator_ShouldInvalidate_WhenKeyIsInvalid()
    {
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var validator = new UpdateProjectCommandValidator(_readOnlyUnitOfWorkMock.Object);

        var command1 = new UpdateProjectCommand(1, string.Empty, "Valid Name", "Valid Description");
        var command2 = new UpdateProjectCommand(1, " ", "Valid Name", "Valid Description");
        var command3 = new UpdateProjectCommand(1, "1ABC", "Valid Name", "Valid Description");
        var command4 = new UpdateProjectCommand(1, "KEYISTOOLONG", "Valid Name", "Valid Description");

        var result1 = await validator.ValidateAsync(command1);
        var result2 = await validator.ValidateAsync(command2);
        var result3 = await validator.ValidateAsync(command3);
        var result4 = await validator.ValidateAsync(command4);

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
    public async Task UpdateProjectCommandValidator_ShouldInvalidate_WhenKeyIsNotUnique()
    {
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.ExistsByKeyAsync("DUPLICATE", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var validator = new UpdateProjectCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new UpdateProjectCommand(1, "DUPLICATE", "Valid Name", "Valid Description");

        var result = await validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Key");
    }

    [Fact]
    public async Task UpdateProjectCommandValidator_ShouldInvalidate_WhenNameIsInvalid()
    {
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.ExistsByKeyAsync("VALID-KEY", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var validator = new UpdateProjectCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new UpdateProjectCommand(1, "VALID-KEY", new string('A', 101), "Valid Description");

        var result = await validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task UpdateProjectCommandValidator_ShouldInvalidate_WhenDescriptionIsInvalid()
    {
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.ExistsByKeyAsync("VALID-KEY", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var validator = new UpdateProjectCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new UpdateProjectCommand(1, "VALID-KEY", "Valid Name", new string('A', 501));

        var result = await validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Description");
    }

    [Fact]
    public async Task DeleteProjectCommandValidator_ShouldValidate()
    {
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var validator = new DeleteProjectCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new DeleteProjectCommand(1);

        var result = await validator.ValidateAsync(command);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task DeleteProjectCommandValidator_ShouldInvalidate_WhenProjectDoesNotExist()
    {
        _readOnlyUnitOfWorkMock
            .Setup(u => u.Projects.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var validator = new DeleteProjectCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new DeleteProjectCommand(1);

        var result = await validator.ValidateAsync(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Id");
    }
}
