using Lucy.Application.Statuses.Commands.DeleteStatus;
using Lucy.Application.Statuses.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Statuses.Commands.DeleteStatus;

public class DeleteStatusCommandValidatorTests : ApplicationTestBase
{
    private readonly Mock<IStatusReadOnlyRepository> _statusRepositoryMock;
    private readonly DeleteStatusCommandValidator _validator;

    public DeleteStatusCommandValidatorTests()
    {
        _statusRepositoryMock = SetupReadOnlyRepository(u => u.Statuses);
        _validator = new DeleteStatusCommandValidator(ReadOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task ValidateAsync_ShouldValidate()
    {
        // Arrange
        _statusRepositoryMock
            .Setup(u => u.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new DeleteStatusCommand(1);

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
            .Setup(u => u.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new DeleteStatusCommand(1);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Id");
    }
}
