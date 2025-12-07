using Lucy.Application.Interfaces;
using Lucy.Application.TicketTags.Commands.AddTicketTag;
using Lucy.Application.TicketTags.Commands.RemoveTicketTag;
using Lucy.Application.TicketTags.Repositories;
using Lucy.Application.Tags.Repositories;
using Lucy.Application.Tickets.Repositories;
using Lucy.Application.Validation;
using Lucy.Domain.Entities;
using Moq;

namespace Lucy.Application.Tests.TicketTags;

public class TicketTagCommandTests
{
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IReadOnlyUnitOfWork> _readOnlyUnitOfWorkMock;
    private readonly Mock<ITicketTagRepository> _ticketTagRepositoryMock;
    private readonly Mock<ITagRepository> _tagRepositoryMock;
    private readonly Mock<ITicketRepository> _ticketRepositoryMock;

    public TicketTagCommandTests()
    {
        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _readOnlyUnitOfWorkMock = new Mock<IReadOnlyUnitOfWork>();
        _ticketTagRepositoryMock = new Mock<ITicketTagRepository>();
        _tagRepositoryMock = new Mock<ITagRepository>();
        _ticketRepositoryMock = new Mock<ITicketRepository>();

        _unitOfWorkMock.Setup(u => u.TicketTags).Returns(_ticketTagRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Tags).Returns(_tagRepositoryMock.Object);
        _unitOfWorkMock.Setup(u => u.Tickets).Returns(_ticketRepositoryMock.Object);

        _readOnlyUnitOfWorkMock.Setup(u => u.Tags).Returns(_tagRepositoryMock.As<ITagReadOnlyRepository>().Object);
        _readOnlyUnitOfWorkMock.Setup(u => u.Tickets).Returns(_ticketRepositoryMock.As<ITicketReadOnlyRepository>().Object);
        _readOnlyUnitOfWorkMock.Setup(u => u.TicketTags).Returns(_ticketTagRepositoryMock.As<ITicketTagReadOnlyRepository>().Object);
    }

    [Fact]
    public async Task AddTicketTagCommandHandler_ShouldAddTicketTag_WhenEntitiesExist()
    {
        var ticket = new Ticket(1, 1, "PROJ-1", 1, "Title") { Id = 10 };
        var tag = new Tag(1, "BUG") { Id = 5 };

        _ticketRepositoryMock
            .Setup(r => r.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticket);
        _tagRepositoryMock
            .Setup(r => r.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(tag);

        var handler = new AddTicketTagCommandHandler(_unitOfWorkMock.Object);
        var command = new AddTicketTagCommand(5, 10);

        await handler.HandleAsync(command, CancellationToken.None);

        _ticketTagRepositoryMock.Verify(
            r => r.AddAsync(
                It.Is<TicketTag>(tt => tt.TagId == 5 && tt.TicketId == 10),
                It.IsAny<CancellationToken>()),
            Times.Once);
        _unitOfWorkMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddTicketTagCommandValidator_ShouldReturnSuccess_WhenTagAndTicketExist()
    {
        _tagRepositoryMock
            .Setup(r => r.ExistsByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _ticketRepositoryMock
            .Setup(r => r.ExistsByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var validator = new AddTicketTagCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new AddTicketTagCommand(5, 10);

        var result = await validator.ValidateAsync(command, CancellationToken.None);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task AddTicketTagCommandValidator_ShouldReturnError_WhenTagNotFound()
    {
        _tagRepositoryMock
            .Setup(r => r.ExistsByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var validator = new AddTicketTagCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new AddTicketTagCommand(5, 10);

        var result = await validator.ValidateAsync(command, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Message == ValidationCode.TagNotFound.ToString());
    }

    [Fact]
    public async Task AddTicketTagCommandValidator_ShouldReturnError_WhenTicketNotFound()
    {
        _tagRepositoryMock
            .Setup(r => r.ExistsByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _ticketRepositoryMock
            .Setup(r => r.ExistsByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var validator = new AddTicketTagCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new AddTicketTagCommand(5, 10);

        var result = await validator.ValidateAsync(command, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Message == ValidationCode.TicketNotFound.ToString());
    }

    [Fact]
    public async Task RemoveTicketTagCommandHandler_ShouldRemoveTicketTag_WhenLinkExists()
    {
        var ticket = new Ticket(1, 1, "PROJ-1", 1, "Title") { Id = 10 };
        var tag = new Tag(1, "BUG") { Id = 5 };
        var ticketTag = new TicketTag(ticket, tag) { Id = 99 };

        _ticketTagRepositoryMock
            .Setup(r => r.GetByTicketAndTagAsync(10, 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ticketTag);

        var handler = new RemoveTicketTagCommandHandler(_unitOfWorkMock.Object);
        var command = new RemoveTicketTagCommand(5, 10);

        await handler.HandleAsync(command, CancellationToken.None);

        _ticketTagRepositoryMock.Verify(r => r.Remove(ticketTag), Times.Once);
        _unitOfWorkMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoveTicketTagCommandHandler_ShouldThrow_WhenLinkNotFound()
    {
        _ticketTagRepositoryMock
            .Setup(r => r.GetByTicketAndTagAsync(10, 5, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TicketTag?)null);

        var handler = new RemoveTicketTagCommandHandler(_unitOfWorkMock.Object);
        var command = new RemoveTicketTagCommand(5, 10);

        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.HandleAsync(command, CancellationToken.None));
    }

    [Fact]
    public async Task RemoveTicketTagCommandValidator_ShouldReturnSuccess_WhenTagAndTicketExist()
    {
        _tagRepositoryMock
            .Setup(r => r.ExistsByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _ticketRepositoryMock
            .Setup(r => r.ExistsByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var validator = new RemoveTicketTagCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new RemoveTicketTagCommand(5, 10);

        var result = await validator.ValidateAsync(command, CancellationToken.None);

        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task RemoveTicketTagCommandValidator_ShouldReturnError_WhenTagNotFound()
    {
        _tagRepositoryMock
            .Setup(r => r.ExistsByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var validator = new RemoveTicketTagCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new RemoveTicketTagCommand(5, 10);

        var result = await validator.ValidateAsync(command, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Message == ValidationCode.TagNotFound.ToString());
    }

    [Fact]
    public async Task RemoveTicketTagCommandValidator_ShouldReturnError_WhenTicketNotFound()
    {
        _tagRepositoryMock
            .Setup(r => r.ExistsByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        _ticketRepositoryMock
            .Setup(r => r.ExistsByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var validator = new RemoveTicketTagCommandValidator(_readOnlyUnitOfWorkMock.Object);
        var command = new RemoveTicketTagCommand(5, 10);

        var result = await validator.ValidateAsync(command, CancellationToken.None);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Message == ValidationCode.TicketNotFound.ToString());
    }
}
