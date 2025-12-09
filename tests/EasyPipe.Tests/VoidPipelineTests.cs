using EasyPipe.Abstractions;
using EasyPipe.Extensions.DependencyInjection;
using EasyPipe.Tests.Services;
using EasyPipe.Tests.Steps;
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

    [Fact]
    public async Task ExecuteAsync_VoidPipeline_ShouldExecuteAllSteps()
    {
        // Arrange
        var logs = new List<string>();
        _fixture.Services.AddScoped<ITestLogger>(_ => new TestLogger(logs));
        _fixture.Services.AddPipeline<TestContext, Unit>(pipeline =>
        {
            pipeline
                .AddStep<VoidStep1>()
                .AddStep<VoidStep2>();
        });
        _fixture.BuildServiceProvider();

        var pipeline = _fixture.Provider.GetRequiredService<IPipeline<TestContext, Unit>>();

        // Act
        var result = await pipeline.ExecuteAsync(new TestContext());

        // Assert
        result.Should().Be(Unit.Value);
        logs.Should().Equal("VoidStep1", "VoidStep2");
    }

    [Fact]
    public async Task ExecuteAsync_VoidPipeline_WhenShortCircuited_ShouldNotExecuteRemaining()
    {
        // Arrange
        var logs = new List<string>();

        _fixture.Services.AddScoped<ITestLogger>(_ => new TestLogger(logs));
        _fixture.Services.AddPipeline<TestContext, Unit>(pipeline =>
        {
            pipeline
                .AddStep<VoidStep1>()
                .AddStep<VoidShortCircuitStep>()
                .AddStep<VoidStep2>();
        });
        _fixture.BuildServiceProvider();

        var pipeline = _fixture.Provider.GetRequiredService<IPipeline<TestContext, Unit>>();

        // Act
        var result = await pipeline.ExecuteAsync(new TestContext());

        // Assert
        result.Should().Be(Unit.Value);
        logs.Should().Equal("VoidStep1", "VoidShortCircuit");
    }
}