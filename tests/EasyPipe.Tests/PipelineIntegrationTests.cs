using EasyPipe.Abstractions;
using EasyPipe.Extensions.DependencyInjection;
using EasyPipe.Tests.Services;
using EasyPipe.Tests.Steps;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace EasyPipe.Tests;

public class PipelineIntegrationTests : IDisposable
{
    private readonly PipelineTestFixture _fixture;

    public PipelineIntegrationTests()
    {
        _fixture = new PipelineTestFixture();
    }

    public void Dispose()
    {
        (_fixture.Provider as ServiceProvider)?.Dispose();
    }

    [Fact]
    public async Task IntegrationTest_ComplexPipelineWithDependencies_ShouldExecuteSuccessfully()
    {
        // Arrange
        var logger = new List<string>();

        _fixture.Services.AddScoped<ITestLogger>(_ => new TestLogger(logger));
        _fixture.Services.AddPipeline<TestContext, TestResult>(pipeline =>
        {
            pipeline
                .AddStep<LoggingStep>()
                .AddStep<ResultStep>();
        });

        _fixture.BuildServiceProvider();
        var pipeline = _fixture.Provider.GetRequiredService<IPipeline<TestContext, TestResult>>();

        // Act
        var result = await pipeline.ExecuteAsync(new TestContext { Value = "test" });

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        logger.Should().Contain("Started");
    }

    [Fact]
    public async Task IntegrationTest_PipelineWithTimeout_ShouldRespectCancellation()
    {
        // Arrange
        _fixture.Services.AddPipeline<TestContext, TestResult>(pipeline =>
        {
            pipeline
                .AddStep<SlowStep>();
        });

        _fixture.BuildServiceProvider();
        var pipeline = _fixture.Provider.GetRequiredService<IPipeline<TestContext, TestResult>>();

        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(50));

        // Act & Assert
        await pipeline.Invoking(p => p.ExecuteAsync(new TestContext(), cts.Token))
            .Should()
            .ThrowAsync<OperationCanceledException>();
    }

    [Fact]
    public async Task IntegrationTest_MultiplePipelinesInContainer_ShouldBeIndependent()
    {
        // Arrange
        var logs = new List<string>();
        _fixture.Services.AddScoped<ITestLogger>(_ => new TestLogger(logs));

        _fixture.Services.AddPipeline<TestContext, TestResult>(pipeline =>
        {
            pipeline
                .AddStep<ResultStep>();
        });

        _fixture.Services.AddPipeline<TestContext, Unit>(pipeline =>
        {
            pipeline
                .AddStep<VoidStep3>();
        });

        _fixture.BuildServiceProvider();

        var typedPipeline = _fixture.Provider.GetRequiredService<IPipeline<TestContext, TestResult>>();
        var voidPipeline = _fixture.Provider.GetRequiredService<IPipeline<TestContext, Unit>>();

        // Act
        var typedResult = await typedPipeline.ExecuteAsync(new TestContext());
        var voidResult = await voidPipeline.ExecuteAsync(new TestContext());

        // Assert
        typedResult.Should().NotBeNull();
        voidResult.Should().Be(Unit.Value);
    }
}