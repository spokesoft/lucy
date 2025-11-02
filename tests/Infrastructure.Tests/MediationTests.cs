using Lucy.Application.Interfaces;
using Lucy.Infrastructure.Mediation;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Lucy.Infrastructure.Tests;

public class MediationTests
{
    private readonly Mock<IServiceProvider> _serviceProviderMock;
    private readonly Mediator _mediator;

    public MediationTests()
    {
        _serviceProviderMock = new Mock<IServiceProvider>();
        _mediator = new Mediator(_serviceProviderMock.Object);
    }

    // Define some test requests and responses
    public record TestRequest : IRequest;
    public record TestRequestWithResponse : IRequest<string>;
    public class TestResponse { public string Message { get; set; } = string.Empty; }
    public record TestRequestWithClassResponse : IRequest<TestResponse>;


    // Define handlers for the test requests
    public class TestRequestHandler : IRequestHandler<TestRequest>
    {
        public Task HandleAsync(TestRequest request, CancellationToken cancellationToken) => Task.CompletedTask;
    }

    public class TestRequestWithResponseHandler : IRequestHandler<TestRequestWithResponse, string>
    {
        public Task<string> HandleAsync(TestRequestWithResponse request, CancellationToken cancellationToken) => Task.FromResult("Success");
    }

    public class TestRequestWithClassResponseHandler : IRequestHandler<TestRequestWithClassResponse, TestResponse>
    {
        public Task<TestResponse> HandleAsync(TestRequestWithClassResponse request, CancellationToken cancellationToken) => Task.FromResult(new TestResponse { Message = "Success" });
    }

    [Fact]
    public async Task Send_ShouldInvokeHandler_ForRequestWithoutResponse()
    {
        // Arrange
        var request = new TestRequest();
        var handler = new TestRequestHandler();

        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IRequestHandler<TestRequest>)))
            .Returns(handler);

        var serviceScope = new Mock<IServiceScope>();

        serviceScope
            .Setup(s => s.ServiceProvider)
            .Returns(_serviceProviderMock.Object);

        var serviceScopeFactory = new Mock<IServiceScopeFactory>();

        serviceScopeFactory
            .Setup(s => s.CreateScope())
            .Returns(serviceScope.Object);

        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IServiceScopeFactory)))
            .Returns(serviceScopeFactory.Object);

        // Act
        await _mediator.Send(request);

        // Assert
        // No assertion needed if the goal is to ensure no exception is thrown.
        // You could add mock verification if the handler had dependencies.
    }

    [Fact]
    public async Task Send_ShouldInvokeHandlerAndReturnResponse_ForRequestWithResponse()
    {
        // Arrange
        var request = new TestRequestWithResponse();
        var handler = new TestRequestWithResponseHandler();

        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IRequestHandler<TestRequestWithResponse, string>)))
            .Returns(handler);

        var serviceScope = new Mock<IServiceScope>();

        serviceScope
            .Setup(s => s.ServiceProvider)
            .Returns(_serviceProviderMock.Object);

        var serviceScopeFactory = new Mock<IServiceScopeFactory>();

        serviceScopeFactory
            .Setup(s => s.CreateScope()).Returns(serviceScope.Object);

        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IServiceScopeFactory)))
            .Returns(serviceScopeFactory.Object);

        // Act
        var response = await _mediator.Send(request);

        // Assert
        Assert.Equal("Success", response);
    }

    [Fact]
    public async Task Send_ShouldInvokeHandlerAndReturnClassResponse_ForRequestWithClassResponse()
    {
        // Arrange
        var request = new TestRequestWithClassResponse();
        var handler = new TestRequestWithClassResponseHandler();

        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IRequestHandler<TestRequestWithClassResponse, TestResponse>)))
            .Returns(handler);

        var serviceScope = new Mock<IServiceScope>();

        serviceScope
            .Setup(s => s.ServiceProvider)
            .Returns(_serviceProviderMock.Object);

        var serviceScopeFactory = new Mock<IServiceScopeFactory>();

        serviceScopeFactory
            .Setup(s => s.CreateScope())
            .Returns(serviceScope.Object);

        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IServiceScopeFactory)))
            .Returns(serviceScopeFactory.Object);

        // Act
        var response = await _mediator.Send(request);

        // Assert
        Assert.Equal("Success", response.Message);
    }

    [Fact]
    public async Task Send_ShouldThrowInvalidOperationException_WhenHandlerNotFound()
    {
        // Arrange
        var request = new TestRequest();

        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IRequestHandler<TestRequest>)))
            .Returns(null!); // Simulate handler not found

        var serviceScope = new Mock<IServiceScope>();

        serviceScope
            .Setup(s => s.ServiceProvider)
            .Returns(_serviceProviderMock.Object);

        var serviceScopeFactory = new Mock<IServiceScopeFactory>();

        serviceScopeFactory
            .Setup(s => s.CreateScope())
            .Returns(serviceScope.Object);

        _serviceProviderMock
            .Setup(sp => sp.GetService(typeof(IServiceScopeFactory)))
            .Returns(serviceScopeFactory.Object);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _mediator.Send(request));
    }

    [Fact]
    public async Task Send_ShouldThrowArgumentNullException_WhenRequestIsNull()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => _mediator.Send<string>(null!));
    }
}
