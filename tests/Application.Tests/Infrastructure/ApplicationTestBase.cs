using Lucy.Application.Common.Interfaces;
using Moq;
using System.Linq.Expressions;

namespace Lucy.Application.Tests.Infrastructure;

public abstract class ApplicationTestBase
{
    protected readonly Mock<IUnitOfWork> UnitOfWorkMock;
    protected readonly Mock<IReadOnlyUnitOfWork> ReadOnlyUnitOfWorkMock;

    protected ApplicationTestBase()
    {
        UnitOfWorkMock = new Mock<IUnitOfWork>();
        ReadOnlyUnitOfWorkMock = new Mock<IReadOnlyUnitOfWork>();
    }

    protected Mock<TRepo> SetupRepository<TRepo>(Expression<Func<IUnitOfWork, TRepo>> selector)
        where TRepo : class
    {
        var mockRepo = new Mock<TRepo>();
        UnitOfWorkMock.Setup(selector).Returns(mockRepo.Object);
        return mockRepo;
    }

    protected Mock<TRepo> SetupReadOnlyRepository<TRepo>(Expression<Func<IReadOnlyUnitOfWork, TRepo>> selector)
        where TRepo : class
    {
        var mockRepo = new Mock<TRepo>();
        ReadOnlyUnitOfWorkMock.Setup(selector).Returns(mockRepo.Object);
        return mockRepo;
    }
}
