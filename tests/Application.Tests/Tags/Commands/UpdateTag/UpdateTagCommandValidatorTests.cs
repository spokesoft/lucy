using Lucy.Application.Tags.Commands.UpdateTag;
using Lucy.Application.Tags.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Lucy.Application.Validation;
using Lucy.Domain.Entities;
using Lucy.Domain.Enums;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Tags.Commands.UpdateTag;

public class UpdateTagCommandValidatorTests : ApplicationTestBase
{
    private readonly Mock<ITagReadOnlyRepository> _tagRepositoryMock;
    private readonly UpdateTagCommandValidator _validator;

    public UpdateTagCommandValidatorTests()
    {
        _tagRepositoryMock = SetupReadOnlyRepository(u => u.Tags);
        _validator = new UpdateTagCommandValidator(ReadOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task ValidateAsync_ShouldReturnSuccess_WhenValidCommandIsGiven()
    {
        // Arrange
        var tag = new Tag(1, "old-key", "Old Tag", "Old Description", Color.Blue);

        _tagRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tag);

        _tagRepositoryMock
            .Setup(repo => repo.ExistsByKeyAsync(1, "NEW-KEY", It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new UpdateTagCommand(1, "new-key", "New Tag", "New Description", null);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_ShouldReturnError_WhenTagDoesNotExist()
    {
        // Arrange
        _tagRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Tag)null!);

        var command = new UpdateTagCommand(1, "new-key", "New Tag", "New Description", null);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Message == ValidationCode.TagNotFound.ToString());
    }

    [Fact]
    public async Task ValidateAsync_ShouldReturnError_WhenKeyAlreadyExists()
    {
        // Arrange
        var tag = new Tag(1, "old-key", "Old Tag", "Old Description", Color.Blue);

        _tagRepositoryMock
            .Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tag);

        _tagRepositoryMock
            .Setup(repo => repo.ExistsByKeyAsync(1, "new-key", It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new UpdateTagCommand(1, "new-key", "New Tag", "New Description", Color.Green);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Message == ValidationCode.TagKeyExists.ToString());
    }
}
