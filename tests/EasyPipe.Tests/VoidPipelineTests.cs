using EasyPipe.Extensions.MicrosoftDependencyInjection.V2;
using EasyPipe.Tests.Services;
using EasyPipe.Tests.Steps;
using EasyPipe.V2;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace EasyPipe.Tests;

public class VoidPipelineTests : IDisposable
{
    private readonly PipelineTestFixture _fixture;

    public VoidPipelineTests()
    {
        _fixture = new PipelineTestFixture();
    }

    public void Dispose()
    {
        (_fixture.Provider as ServiceProvider)?.Dispose();
    }

    /// <summary>
    /// TEST: Void pipeline should execute steps
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_VoidPipeline_ShouldExecuteAllSteps()
    {
        // Arrange
        var callOrder = new List<string>();
        _fixture.Services.AddScoped<CallTracker>(_ => new CallTracker(callOrder));
        _fixture.Services.AddPipeline<TestContext, Unit>()
            .AddStep<VoidStep1>()
            .AddStep<VoidStep2>()
            .Build();
        _fixture.BuildServiceProvider();

        var pipeline = _fixture.Provider.GetRequiredService<IPipeline<TestContext, Unit>>();

        // Act
        var result = await pipeline.ExecuteAsync(new TestContext());

        // Assert
        result.Should().Be(Unit.Value);
        callOrder.Should().Equal("VoidStep1", "VoidStep2");
    }

    /// <summary>
    /// TEST: Void pipeline should support short-circuiting
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_VoidPipeline_WhenShortCircuited_ShouldNotExecuteRemaining()
    {
        // Arrange
        var callOrder = new List<string>();

        _fixture.Services.AddScoped<CallTracker>(_ => new CallTracker(callOrder));
        _fixture.Services.AddPipeline<TestContext, Unit>()
            .AddStep<VoidStep1>()
            .AddStep<VoidShortCircuitStep>()
            .AddStep<VoidStep2>()
            .Build();
        _fixture.BuildServiceProvider();

        var pipeline = _fixture.Provider.GetRequiredService<IPipeline<TestContext, Unit>>();

        // Act
        var result = await pipeline.ExecuteAsync(new TestContext());

        // Assert
        result.Should().Be(Unit.Value);
        callOrder.Should().Equal("VoidStep1", "VoidShortCircuit");
    }
}