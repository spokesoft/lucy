using Lucy.Application.Comments.Queries.ListProjectComments;
using Lucy.Application.Projects.Repositories;
using Lucy.Application.Tests.Infrastructure;
using Moq;
using Xunit;

namespace Lucy.Application.Tests.Comments.Queries.ListProjectComments;

public class ListProjectCommentsQueryValidatorTests : ApplicationTestBase
{
    private readonly Mock<IProjectReadOnlyRepository> _projectRepositoryMock;
    private readonly ListProjectCommentsQueryValidator _validator;

    public ListProjectCommentsQueryValidatorTests()
    {
        _projectRepositoryMock = SetupReadOnlyRepository(u => u.Projects);
        _validator = new ListProjectCommentsQueryValidator(ReadOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task ValidateAsync_ShouldValidate()
    {
        // Arrange
        _projectRepositoryMock
            .Setup(u => u.ExistsByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var query = new ListProjectCommentsQuery(1);

        // Act
        var result = await _validator.ValidateAsync(query);

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

        var query = new ListProjectCommentsQuery(1);

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ProjectId");
    }
}
