using Lucy.Application.Interfaces;
using Lucy.Application.TicketTags.Commands.AddTicketTag;
using Lucy.Application.TicketTags.Repositories;
using Lucy.Application.Tags.Repositories;
using Lucy.Application.Tickets.Repositories;
using Lucy.Application.Validation;
using Moq;

namespace Lucy.Application.Tests.TicketTags.Commands.AddTicketTag;

public class AddTicketTagCommandValidatorTests
{
    private readonly Mock<IReadOnlyUnitOfWork> _readOnlyUnitOfWorkMock;
    private readonly Mock<ITagReadOnlyRepository> _tagRepositoryMock;
    private readonly Mock<ITicketReadOnlyRepository> _ticketRepositoryMock;
    private readonly AddTicketTagCommandValidator _validator;

    public AddTicketTagCommandValidatorTests()
    {
        _readOnlyUnitOfWorkMock = new Mock<IReadOnlyUnitOfWork>();
        _tagRepositoryMock = new Mock<ITagReadOnlyRepository>();
        _ticketRepositoryMock = new Mock<ITicketReadOnlyRepository>();

        _readOnlyUnitOfWorkMock.Setup(u => u.Tags).Returns(_tagRepositoryMock.Object);
        _readOnlyUnitOfWorkMock.Setup(u => u.Tickets).Returns(_ticketRepositoryMock.Object);

        _validator = new AddTicketTagCommandValidator(_readOnlyUnitOfWorkMock.Object);
    }

    [Fact]
    public async Task ValidateAsync_ShouldReturnSuccess_WhenTagAndTicketExist()
    {
        // Arrange
        _tagRepositoryMock
            .Setup(r => r.ExistsByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _ticketRepositoryMock
            .Setup(r => r.ExistsByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new AddTicketTagCommand(5, 10);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken.None);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task ValidateAsync_ShouldReturnError_WhenTagNotFound()
    {
        // Arrange
        _tagRepositoryMock
            .Setup(r => r.ExistsByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new AddTicketTagCommand(5, 10);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Message == ValidationCode.TagNotFound.ToString());
    }

    [Fact]
    public async Task ValidateAsync_ShouldReturnError_WhenTicketNotFound()
    {
        // Arrange
        _tagRepositoryMock
            .Setup(r => r.ExistsByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _ticketRepositoryMock
            .Setup(r => r.ExistsByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var command = new AddTicketTagCommand(5, 10);

        // Act
        var result = await _validator.ValidateAsync(command, CancellationToken.None);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Message == ValidationCode.TicketNotFound.ToString());
    }
}
