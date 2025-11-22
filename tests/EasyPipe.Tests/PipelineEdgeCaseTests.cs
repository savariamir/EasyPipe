using System.Diagnostics;
using EasyPipe.Extensions.MicrosoftDependencyInjection.V2;
using EasyPipe.Tests.Steps;
using EasyPipe.V2;
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

    /// <summary>
    /// TEST: Pipeline with single step
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_WithSingleStep_ShouldExecute()
    {
        // Arrange
        _fixture.Services.AddPipeline<TestContext, TestResult>()
            .AddStep<ResultStep>()
            .Build();

        _fixture.BuildServiceProvider();
        var pipeline = _fixture.Provider.GetRequiredService<IPipeline<TestContext, TestResult>>();

        // Act
        var result = await pipeline.ExecuteAsync(new TestContext());

        // Assert
        result.ResultValue.Should().Be("completed");
    }

    /// <summary>
    /// TEST: Pipeline step that returns modified context
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_WithContextModification_ShouldPreserveMutations()
    {
        // Arrange
        _fixture.Services.AddPipeline<TestContext, TestResult>()
            .AddStep<ContextModifyingStep>()
            .AddStep<ContextValidatingStep>()
            .Build();

        _fixture.BuildServiceProvider();
        var pipeline = _fixture.Provider.GetRequiredService<IPipeline<TestContext, TestResult>>();

        var context = new TestContext { Value = "original" };

        // Act
        await pipeline.ExecuteAsync(context);

        // Assert
        context.Value.Should().Be("modified");
    }

    /// <summary>
    /// TEST: Pipeline with null context should work if allowed by step
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_WithNullContext_StepsShouldHandleIt()
    {
        // Arrange
        _fixture.Services.AddPipeline<TestContext?, TestResult>()
            .AddStep<NullableContextStep>()
            .Build();

        _fixture.BuildServiceProvider();
        var pipeline = _fixture.Provider.GetRequiredService<IPipeline<TestContext?, TestResult>>();

        // Act & Assert
        await pipeline.Invoking(p => p.ExecuteAsync(null))
            .Should()
            .NotThrowAsync();
    }

    /// <summary>
    /// TEST: Step that returns Task.CompletedTask correctly
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_StepReturningCompletedTask_ShouldWork()
    {
        // Arrange
        _fixture.Services.AddPipeline<TestContext, TestResult>()
            .AddStep<CompletedTaskStep>()
            .Build();

        _fixture.BuildServiceProvider();
        var pipeline = _fixture.Provider.GetRequiredService<IPipeline<TestContext, TestResult>>();

        // Act & Assert
        await pipeline.Invoking(p => p.ExecuteAsync(new TestContext()))
            .Should()
            .NotThrowAsync();
    }

    /// <summary>
    /// TEST: Pipeline should handle step with async exception
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_WithAsyncException_ShouldPropagate()
    {
        // Arrange
        _fixture.Services.AddPipeline<TestContext, TestResult>()
            .AddStep<AsyncThrowingStep>()
            .Build();

        _fixture.BuildServiceProvider();
        var pipeline = _fixture.Provider.GetRequiredService<IPipeline<TestContext, TestResult>>();

        // Act & Assert
        await pipeline.Invoking(p => p.ExecuteAsync(new TestContext()))
            .Should()
            .ThrowAsync<InvalidOperationException>();
    }

    /// <summary>
    /// TEST: Pipeline with step that delays before calling next
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_WithDelayedNext_ShouldWaitForCompletion()
    {
        // Arrange
        _fixture.Services.AddPipeline<TestContext, TestResult>()
            .AddStep<DelayedStep>()
            .Build();

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