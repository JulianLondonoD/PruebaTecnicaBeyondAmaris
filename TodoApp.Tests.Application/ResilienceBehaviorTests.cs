using Microsoft.Extensions.Logging;
using Moq;
using Polly;
using TodoApp.Application.Behaviors;
using TodoApp.Application.Common.Interfaces;
using MediatR;

namespace TodoApp.Tests.Application;

public class ResilienceBehaviorTests
{
    private readonly Mock<ILogger<ResilienceBehavior<TestCommand, string>>> _mockLogger;
    private readonly Mock<IResiliencePolicyProvider> _mockPolicyProvider;
    private readonly ResilienceBehavior<TestCommand, string> _behavior;

    public ResilienceBehaviorTests()
    {
        _mockLogger = new Mock<ILogger<ResilienceBehavior<TestCommand, string>>>();
        _mockPolicyProvider = new Mock<IResiliencePolicyProvider>();
        _behavior = new ResilienceBehavior<TestCommand, string>(_mockLogger.Object, _mockPolicyProvider.Object);
    }

    [Fact]
    public async Task Handle_CommandRequest_AppliesCommandPolicy()
    {
        // Arrange
        var command = new TestCommand();
        var expectedResult = "success";
        var policy = CreatePassthroughPolicy<string>();
        
        _mockPolicyProvider
            .Setup(x => x.GetCommandPolicy<string>())
            .Returns(policy);

        RequestHandlerDelegate<string> next = () => Task.FromResult(expectedResult);

        // Act
        var result = await _behavior.Handle(command, next, CancellationToken.None);

        // Assert
        Assert.Equal(expectedResult, result);
        _mockPolicyProvider.Verify(x => x.GetCommandPolicy<string>(), Times.Once);
    }

    [Fact]
    public async Task Handle_QueryRequest_AppliesQueryPolicy()
    {
        // Arrange
        var query = new TestQuery();
        var expectedResult = "query result";
        var policy = CreatePassthroughPolicy<string>();
        
        var queryLogger = new Mock<ILogger<ResilienceBehavior<TestQuery, string>>>();
        var queryBehavior = new ResilienceBehavior<TestQuery, string>(queryLogger.Object, _mockPolicyProvider.Object);
        
        _mockPolicyProvider
            .Setup(x => x.GetQueryPolicy<string>())
            .Returns(policy);

        RequestHandlerDelegate<string> next = () => Task.FromResult(expectedResult);

        // Act
        var result = await queryBehavior.Handle(query, next, CancellationToken.None);

        // Assert
        Assert.Equal(expectedResult, result);
        _mockPolicyProvider.Verify(x => x.GetQueryPolicy<string>(), Times.Once);
    }

    [Fact]
    public async Task Handle_NonCommandNonQuery_SkipsResilience()
    {
        // Arrange
        var otherRequest = new TestOtherRequest();
        var expectedResult = "other result";
        
        var otherLogger = new Mock<ILogger<ResilienceBehavior<TestOtherRequest, string>>>();
        var otherBehavior = new ResilienceBehavior<TestOtherRequest, string>(otherLogger.Object, _mockPolicyProvider.Object);

        RequestHandlerDelegate<string> next = () => Task.FromResult(expectedResult);

        // Act
        var result = await otherBehavior.Handle(otherRequest, next, CancellationToken.None);

        // Assert
        Assert.Equal(expectedResult, result);
        _mockPolicyProvider.Verify(x => x.GetCommandPolicy<string>(), Times.Never);
        _mockPolicyProvider.Verify(x => x.GetQueryPolicy<string>(), Times.Never);
    }

    private static ResiliencePipeline<T> CreatePassthroughPolicy<T>()
    {
        return new ResiliencePipelineBuilder<T>().Build();
    }

    // Test request types
    public record TestCommand : IRequest<string>;
    public record TestQuery : IRequest<string>;
    public record TestOtherRequest : IRequest<string>;
}
