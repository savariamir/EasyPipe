using EasyPipe.Extensions.MicrosoftDependencyInjection.V2;
using EasyPipe.Tests.Steps;
using EasyPipe.V2;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace EasyPipe.Tests;

public class PipelineExecutionTests : IDisposable
{
    private readonly PipelineTestFixture _fixture;

    public PipelineExecutionTests()
    {
        _fixture = new PipelineTestFixture();
    }

    public void Dispose()
    {
        (_fixture.Provider as ServiceProvider)?.Dispose();
    }


    [Fact]
    public async Task ExecuteAsync_WithMultipleSteps_ShouldExecuteInOrder()
    {
        // Arrange
        _fixture.Services.AddPipeline<TestContext, TestResult>()
            .AddStep<FirstStep>()
            .AddStep<SecondStep>()
            .AddStep<ThirdStep>()
            .Build();
        _fixture.BuildServiceProvider();

        var pipeline = _fixture.Provider.GetRequiredService<IPipeline<TestContext, TestResult>>();
        var context = new TestContext();

        // Act
        await pipeline.ExecuteAsync(context);

        // Assert
        context.ExecutedSteps.Should().Equal("Step1", "Step2", "Step3");
    }


    [Fact]
    public async Task ExecuteAsync_ShouldPassContextThroughSteps()
    {
        // Arrange
        _fixture.Services.AddPipeline<TestContext, TestResult>()
            .AddStep<ContextModifyingStep>()
            .Build();
        _fixture.BuildServiceProvider();

        var pipeline = _fixture.Provider.GetRequiredService<IPipeline<TestContext, TestResult>>();
        var context = new TestContext { Value = "initial" };

        // Act
        await pipeline.ExecuteAsync(context);

        // Assert
        context.Value.Should().Be("modified");
    }

    /// <summary>
    /// TEST: Pipeline should return result from final step
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_ShouldReturnResultFromFinalStep()
    {
        // Arrange
        _fixture.Services.AddPipeline<TestContext, TestResult>()
            .AddStep<ResultStep>()
            .Build();
        _fixture.BuildServiceProvider();

        var pipeline = _fixture.Provider.GetRequiredService<IPipeline<TestContext, TestResult>>();
        var context = new TestContext();

        // Act
        var result = await pipeline.ExecuteAsync(context);

        // Assert
        result.Should().NotBeNull();
        result.ResultValue.Should().Be("completed");
        result.Success.Should().BeTrue();
    }


    [Fact]
    public async Task ExecuteAsync_WhenStepThrows_ShouldPropagateException()
    {
        // Arrange
        _fixture.Services.AddPipeline<TestContext, TestResult>()
            .AddStep<ThrowingStep>()
            .Build();
        _fixture.BuildServiceProvider();

        var pipeline = _fixture.Provider.GetRequiredService<IPipeline<TestContext, TestResult>>();

        // Act & Assert
        await pipeline.Invoking(p => p.ExecuteAsync(new TestContext()))
            .Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Step failed");
    }


    [Fact]
    public async Task ExecuteAsync_WhenMiddleStepThrows_ShouldNotExecuteRemainingSteps()
    {
        // Arrange
        _fixture.Services.AddPipeline<TestContext, TestResult>()
            .AddStep<FirstStep>()
            .AddStep<ThrowingStep>()
            .AddStep<ThirdStep>()
            .Build();
        _fixture.BuildServiceProvider();

        var pipeline = _fixture.Provider.GetRequiredService<IPipeline<TestContext, TestResult>>();
        var context = new TestContext();

        // Act & Assert
        await pipeline.Invoking(p => p.ExecuteAsync(context))
            .Should()
            .ThrowAsync<InvalidOperationException>();

        context.ExecutedSteps.Should().Equal("Step1");
    }

    /// <summary>
    /// TEST: Pipeline should short-circuit when step doesn't call next
    /// </summary>
    [Fact]
    public async Task ExecuteAsync_WhenStepDoesNotCallNext_ShouldShortCircuit()
    {
        // Arrange
        _fixture.Services.AddPipeline<TestContext, TestResult>()
            .AddStep<FirstStep>()
            .AddStep<ShortCircuitStep>()
            .AddStep<ThirdStep>()
            .Build();
        _fixture.BuildServiceProvider();

        var pipeline = _fixture.Provider.GetRequiredService<IPipeline<TestContext, TestResult>>();
        var context = new TestContext();

        // Act
        var result = await pipeline.ExecuteAsync(context);

        // Assert
        context.ExecutedSteps.Should().Equal("Step1", "ShortCircuit");
        result.ResultValue.Should().Be("short_circuit_result");
    }


    [Fact]
    public async Task ExecuteAsync_WhenCancelled_ShouldThrowOperationCanceledException()
    {
        // Arrange
        _fixture.Services.AddPipeline<TestContext, TestResult>()
            .AddStep<CancellationAwareStep>()
            .Build();
        _fixture.BuildServiceProvider();

        var pipeline = _fixture.Provider.GetRequiredService<IPipeline<TestContext, TestResult>>();
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();

        // Act & Assert
        await pipeline.Invoking(p => p.ExecuteAsync(new TestContext(), cts.Token))
            .Should()
            .ThrowAsync<OperationCanceledException>();
    }


    [Fact]
    public async Task ExecuteAsync_ShouldPassCancellationTokenToSteps()
    {
        // Arrange
        _fixture.Services.AddPipeline<TestContext, TestResult>()
            .AddStep<CancellationTokenCapturingStep>()
            .Build();
        _fixture.BuildServiceProvider();

        var pipeline = _fixture.Provider.GetRequiredService<IPipeline<TestContext, TestResult>>();
        using var cts = new CancellationTokenSource();

        // Act
        var context = new TestContext();
        await pipeline.ExecuteAsync(context, cts.Token);

        // Assert
        context.UsedCancellationToken.Should().NotBeNull();
        context.UsedCancellationToken!.Value.Should().Be(cts.Token);
    }


    [Fact]
    public async Task ExecuteAsync_WithDiagnostics_ShouldTrackExecutionTimings()
    {
        // Arrange
        var diagnosticsMock = new Mock<IPipelineDiagnostics>();
        _fixture.Services.AddSingleton(diagnosticsMock.Object);
        _fixture.Services.AddPipeline<TestContext, TestResult>()
            .AddStep<FirstStep>()
            .Build();
        _fixture.BuildServiceProvider();

        var pipeline = _fixture.Provider.GetRequiredService<IPipeline<TestContext, TestResult>>();

        // Act
        await pipeline.ExecuteAsync(new TestContext());

        // Assert
        diagnosticsMock.Verify(d => d.OnStepStarting(typeof(FirstStep), 0), Times.Once);
        diagnosticsMock.Verify(d => d.OnStepCompleted(
            typeof(FirstStep), 0, It.Is<TimeSpan>(ts => ts.TotalMilliseconds >= 0)), Times.Once);
        diagnosticsMock.Verify(d => d.OnPipelineCompleted(
            It.Is<TimeSpan>(ts => ts.TotalMilliseconds >= 0), true), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WhenStepThrowsException_DiagnosticsShouldRecordFailure()
    {
        // Arrange
        var diagnosticsMock = new Mock<IPipelineDiagnostics>();
        _fixture.Services.AddSingleton(diagnosticsMock.Object);
        _fixture.Services.AddPipeline<TestContext, TestResult>()
            .AddStep<ThrowingStep>()
            .Build();
        _fixture.BuildServiceProvider();

        var pipeline = _fixture.Provider.GetRequiredService<IPipeline<TestContext, TestResult>>();

        // Act & Assert
        await pipeline.Invoking(p => p.ExecuteAsync(new TestContext()))
            .Should()
            .ThrowAsync<InvalidOperationException>();

        diagnosticsMock.Verify(d => d.OnPipelineCompleted(
            It.IsAny<TimeSpan>(), false), Times.Once); // false = failed
    }
}