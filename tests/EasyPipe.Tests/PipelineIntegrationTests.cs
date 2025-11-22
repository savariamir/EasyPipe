using EasyPipe.Extensions.MicrosoftDependencyInjection.V2;
using EasyPipe.Tests.Services;
using EasyPipe.Tests.Steps;
using EasyPipe.V2;
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
        _fixture.Services.AddScoped<ITestRepository>(_ => new TestRepository());

        _fixture.Services.AddPipeline<TestContext, TestResult>()
            .AddStep<LoggingStep>()
            .AddStep<ValidationStep>()
            .AddStep<ProcessingStep>()
            .AddStep<ResultStep>()
            .Build();

        _fixture.BuildServiceProvider();
        var pipeline = _fixture.Provider.GetRequiredService<IPipeline<TestContext, TestResult>>();

        // Act
        var result = await pipeline.ExecuteAsync(new TestContext { Value = "test" });

        // Assert
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
        logger.Should().Contain("Started");
        logger.Should().Contain("Validated");
        logger.Should().Contain("Processed");
    }

    [Fact]
    public async Task IntegrationTest_PipelineWithTimeout_ShouldRespectCancellation()
    {
        // Arrange
        _fixture.Services.AddPipeline<TestContext, TestResult>()
            .AddStep<SlowStep>()
            .Build();

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
        var callOrder = new List<string>();
        _fixture.Services.AddScoped<CallTracker>(_ => new CallTracker(callOrder));

        _fixture.Services.AddPipeline<TestContext, TestResult>()
            .AddStep<ResultStep>()
            .Build();

        _fixture.Services.AddPipeline<EventContext, Unit>()
            .AddStep<VoidStep3>()
            .Build();

        _fixture.BuildServiceProvider();

        var typedPipeline = _fixture.Provider.GetRequiredService<IPipeline<TestContext, TestResult>>();
        var voidPipeline = _fixture.Provider.GetRequiredService<IPipeline<EventContext, Unit>>();

        // Act
        var typedResult = await typedPipeline.ExecuteAsync(new TestContext());
        var voidResult = await voidPipeline.ExecuteAsync(new EventContext());

        // Assert
        typedResult.Should().NotBeNull();
        voidResult.Should().Be(Unit.Value);
    }

    [Fact]
    public async Task IntegrationTest_ContextPropagation_ShouldAllowDataSharing()
    {
        // Arrange
        _fixture.Services.AddPipeline<TestContext, TestResult>()
            .AddStep<EnrichmentStep1>()
            .AddStep<EnrichmentStep2>()
            .Build();

        _fixture.BuildServiceProvider();
        var pipeline = _fixture.Provider.GetRequiredService<IPipeline<TestContext, TestResult>>();

        var context = new TestContext { Value = "initial" };

        // Act
        await pipeline.ExecuteAsync(context);

        // Assert
        context.Value.Should().Contain("initial_enriched1_enriched2");
    }
}