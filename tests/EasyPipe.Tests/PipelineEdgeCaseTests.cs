using System.Diagnostics;
using EasyPipe.Abstractions;
using EasyPipe.Extensions.DependencyInjection;
using EasyPipe.Tests.Steps;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace EasyPipe.Tests;

public class PipelineEdgeCaseTests : IDisposable
{
    private readonly PipelineTestFixture _fixture;

    public PipelineEdgeCaseTests()
    {
        _fixture = new PipelineTestFixture();
    }

    public void Dispose()
    {
        (_fixture.Provider as ServiceProvider)?.Dispose();
    }


    [Fact]
    public async Task ExecuteAsync_WithSingleStep_ShouldExecute()
    {
        // Arrange

        _fixture.Services.AddPipeline<TestContext, TestResult>(pipeline => { pipeline.AddStep<ResultStep>(); });

        _fixture.BuildServiceProvider();
        var pipeline = _fixture.Provider.GetRequiredService<IPipeline<TestContext, TestResult>>();

        // Act
        var result = await pipeline.ExecuteAsync(new TestContext());

        // Assert
        result.ResultValue.Should().Be("completed");
    }

    [Fact]
    public async Task ExecuteAsync_WithContextModification_ShouldPreserveMutations()
    {
        // Arrange
        _fixture.Services.AddPipeline<TestContext, TestResult>(pipeline =>
        {
            pipeline
                .AddStep<ContextModifyingStep>()
                .AddStep<ContextValidatingStep>();
        });

        _fixture.BuildServiceProvider();
        var pipeline = _fixture.Provider.GetRequiredService<IPipeline<TestContext, TestResult>>();

        var context = new TestContext { Value = "original" };

        // Act
        await pipeline.ExecuteAsync(context);

        // Assert
        context.Value.Should().Be("modified");
    }


    [Fact]
    public async Task ExecuteAsync_WithNullContext_StepsShouldHandleIt()
    {
        // Arrange
        _fixture.Services.AddPipeline<TestContext?, TestResult>(pipeline =>
        {
            pipeline
                .AddStep<NullableContextStep>();
        });

        _fixture.BuildServiceProvider();
        var pipeline = _fixture.Provider.GetRequiredService<IPipeline<TestContext?, TestResult>>();

        // Act & Assert
        await pipeline.Invoking(p => p.ExecuteAsync(null))
            .Should()
            .NotThrowAsync();
    }

    [Fact]
    public async Task ExecuteAsync_StepReturningCompletedTask_ShouldWork()
    {
        // Arrange
        _fixture.Services.AddPipeline<TestContext, TestResult>(pipeline =>
        {
            pipeline
                .AddStep<CompletedTaskStep>();
        });

        _fixture.BuildServiceProvider();
        var pipeline = _fixture.Provider.GetRequiredService<IPipeline<TestContext, TestResult>>();

        // Act & Assert
        await pipeline.Invoking(p => p.ExecuteAsync(new TestContext()))
            .Should()
            .NotThrowAsync();
    }

    [Fact]
    public async Task ExecuteAsync_WithDelayedNext_ShouldWaitForCompletion()
    {
        // Arrange
        _fixture.Services.AddPipeline<TestContext, TestResult>(pipeline => pipeline
            .AddStep<DelayedStep>());

        _fixture.BuildServiceProvider();
        var pipeline = _fixture.Provider.GetRequiredService<IPipeline<TestContext, TestResult>>();

        var sw = Stopwatch.StartNew();

        // Act
        await pipeline.ExecuteAsync(new TestContext());
        sw.Stop();

        // Assert
        sw.ElapsedMilliseconds.Should().BeGreaterThanOrEqualTo(100);
    }
}