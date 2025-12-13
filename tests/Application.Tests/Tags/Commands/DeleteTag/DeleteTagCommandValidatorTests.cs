using Lucy.Application.Tags.Commands.DeleteTag;
using Lucy.Application.Tags.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Lucy.Application.Validation;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Tags.Commands.DeleteTag;

public class DeleteTagCommandValidatorTests : ApplicationTestBase
{
    private readonly Mock<ITagReadOnlyRepository> _tagRepositoryMock;
    private readonly DeleteTagCommandValidator _validator;

    public DeleteTagCommandValidatorTests()
    {
        _tagRepositoryMock = SetupReadOnlyRepository(u => u.Tags);
        _validator = new DeleteTagCommandValidator(ReadOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task ValidateAsync_ShouldReturnSuccess_WhenTagExists()
    {
        // Arrange
        _tagRepositoryMock
            .Setup(repo => repo.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new DeleteTagCommand(1);

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
            .Setup(repo => repo.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new DeleteTagCommand(1);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Message == ValidationCode.TagNotFound.ToString());
    }
}
