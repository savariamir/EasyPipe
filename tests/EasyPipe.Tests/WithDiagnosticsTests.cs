using AwesomeAssertions;
using EasyPipe.Abstractions;
using EasyPipe.Extensions.DependencyInjection;
using EasyPipe.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace EasyPipe.Tests;

public class WithDiagnosticsTests
{
    private class TestContext
    {
        public int Value { get; set; }
    }

    private class TestResponse
    {
        public int Result { get; set; }
    }
    
    private class Step1 : IPipelineStep<TestContext, TestResponse>
    {
        public async Task<TestResponse> RunAsync(
            TestContext context,
            PipeDelegate<TestContext, TestResponse> next,
            CancellationToken ct = default)
        {
            context.Value += 10;
            await Task.Delay(10, ct);
            return await next(context, ct);
        }
    }

    private class Step2 : IPipelineStep<TestContext, TestResponse>
    {
        public async Task<TestResponse> RunAsync(
            TestContext context,
            PipeDelegate<TestContext, TestResponse> next,
            CancellationToken ct = default)
        {
            context.Value += 20;
            return await next(context, ct);
        }
    }

    private class FinalStep : IPipelineStep<TestContext, TestResponse>
    {
        public Task<TestResponse> RunAsync(
            TestContext context,
            PipeDelegate<TestContext, TestResponse> next,
            CancellationToken ct = default)
        {
            return Task.FromResult(new TestResponse { Result = context.Value });
        }
    }

    // Test diagnostics implementations
    private class TestDiagnostics : IPipelineDiagnostics
    {
        public List<string> Events { get; } = new();

        public void OnStepStarting(Type stepType, int stepIndex)
        {
            Events.Add($"starting_{stepIndex}_{stepType.Name}");
        }

        public void OnStepCompleted(Type stepType, int stepIndex, TimeSpan duration)
        {
            Events.Add($"completed_{stepIndex}_{stepType.Name}");
        }

        public void OnStepFailed(Type stepType, int stepIndex, Exception exception, TimeSpan duration)
        {
            Events.Add($"failed_{stepIndex}_{stepType.Name}_{exception.GetType().Name}");
        }

        public void OnPipelineCompleted(TimeSpan duration, bool success)
        {
            Events.Add($"pipeline_completed_{success}");
        }
    }

    private class DependencyInjectedDiagnostics : IPipelineDiagnostics
    {
        public readonly TestDependency _dependency;
        public List<string> Events { get; } = new();

        public DependencyInjectedDiagnostics(TestDependency dependency)
        {
            _dependency = dependency ?? throw new ArgumentNullException(nameof(dependency));
        }

        public void OnStepStarting(Type stepType, int stepIndex)
        {
            Events.Add($"starting_{stepIndex}");
            _dependency.CallCount++;
        }

        public void OnStepCompleted(Type stepType, int stepIndex, TimeSpan duration)
        {
            Events.Add($"completed_{stepIndex}");
        }

        public void OnStepFailed(Type stepType, int stepIndex, Exception exception, TimeSpan duration)
        {
            Events.Add($"failed_{stepIndex}");
        }

        public void OnPipelineCompleted(TimeSpan duration, bool success)
        {
            Events.Add($"pipeline_{success}");
        }
    }

    private class TestDependency
    {
        public int CallCount { get; set; }
    }

    [Fact]
    public async Task WithDiagnostics_TypeBased_RegistersDiagnosticsInContainer()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddPipeline<TestContext, TestResponse>(pipeline =>
        {
            pipeline
                .AddStep<Step1>()
                .AddStep<Step2>()
                .AddStep<FinalStep>()
                .WithDiagnostics<TestDiagnostics>()
                ;
        });

        var provider = services.BuildServiceProvider();

        // Act
        var diagnostics = provider.GetService<IPipelineDiagnostics>();

        // Assert
        diagnostics
            .Should()
            .NotBeNull()
            .And.BeOfType<TestDiagnostics>();
    }

    [Fact]
    public async Task WithDiagnostics_InstanceBased_RegistersDiagnosticsInContainer()
    {
        // Arrange
        var diagnosticsInstance = new TestDiagnostics();
        var services = new ServiceCollection();
        services.AddPipeline<TestContext, TestResponse>(pipeline =>
        {
            pipeline
                .AddStep<Step1>()
                .AddStep<Step2>()
                .AddStep<FinalStep>()
                .WithDiagnostics(diagnosticsInstance);
        });

        var provider = services.BuildServiceProvider();

        // Act
        var diagnostics = provider.GetService<IPipelineDiagnostics>();

        // Assert
        diagnostics
            .Should()
            .BeSameAs(diagnosticsInstance);
    }

    [Fact]
    public async Task WithDiagnostics_CapturesStepExecution()
    {
        // Arrange
        var diagnostics = new TestDiagnostics();
        var services = new ServiceCollection();
        services.AddPipeline<TestContext, TestResponse>(pipeline =>
        {
            pipeline
                .AddStep<Step1>()
                .AddStep<Step2>()
                .AddStep<FinalStep>()
                .WithDiagnostics(diagnostics);
        });

        var provider = services.BuildServiceProvider();
        var pipeline = provider.GetRequiredService<IPipeline<TestContext, TestResponse>>();

        // Act
        var result = await pipeline.ExecuteAsync(new TestContext { Value = 0 });

        // Assert
        result.Result
            .Should()
            .Be(30);

        diagnostics.Events
            .Should()
            .Contain(new[]
            {
                "starting_0_Step1",
                "completed_0_Step1",
                "starting_1_Step2",
                "completed_1_Step2",
                "starting_2_FinalStep",
                "completed_2_FinalStep",
                "pipeline_completed_True"
            });
    }

    [Fact]
    public async Task WithDiagnostics_CapturesPipelineCompletion()
    {
        // Arrange
        var diagnostics = new TestDiagnostics();
        var services = new ServiceCollection();
        services.AddPipeline<TestContext, TestResponse>(pipeline =>
        {
            pipeline
                .AddStep<Step1>()
                .AddStep<FinalStep>()
                .WithDiagnostics(diagnostics);
        });

        var provider = services.BuildServiceProvider();
        var pipeline = provider.GetRequiredService<IPipeline<TestContext, TestResponse>>();

        // Act
        await pipeline.ExecuteAsync(new TestContext());

        // Assert
        diagnostics.Events
            .Should()
            .NotBeEmpty()
            .And.Contain("pipeline_completed_True");
    }

    [Fact]
    public async Task WithDiagnostics_CapturesFailure()
    {
        // Arrange
        var diagnostics = new TestDiagnostics();

        var services = new ServiceCollection();
        services.AddScoped<FailingStep>();
        services.AddPipeline<TestContext, TestResponse>(pipeline =>
        {
            pipeline
                .AddStep<Step1>()
                .AddStep<FailingStep>()
                .WithDiagnostics(diagnostics)
                ;
        });

        var provider = services.BuildServiceProvider();
        var pipeline = provider.GetRequiredService<IPipeline<TestContext, TestResponse>>();

        // Act & Assert
        await pipeline
            .Invoking(p => p.ExecuteAsync(new TestContext()))
            .Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Step failed intentionally");

        diagnostics.Events
            .Should()
            .Contain("failed_1_FailingStep_InvalidOperationException")
            .And.Contain("pipeline_completed_False");
    }

    [Fact]
    public async Task WithDiagnostics_TypeBased_SupportsDependencyInjection()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddScoped<TestDependency>();
        services.AddPipeline<TestContext, TestResponse>(pipeline =>
        {
            pipeline
                .AddStep<Step1>()
                .AddStep<Step2>()
                .AddStep<FinalStep>()
                .WithDiagnostics<DependencyInjectedDiagnostics>()
                ;
        });

        var provider = services.BuildServiceProvider();
        var pipeline = provider.GetRequiredService<IPipeline<TestContext, TestResponse>>();

        // Act
        await pipeline.ExecuteAsync(new TestContext());

        // Assert
        var diagnostics = provider.GetRequiredService<IPipelineDiagnostics>();
        diagnostics
            .Should()
            .BeOfType<DependencyInjectedDiagnostics>();

        var injectedDiagnostics = (DependencyInjectedDiagnostics)diagnostics;
        injectedDiagnostics._dependency.CallCount
            .Should()
            .BeGreaterThan(0);
    }

    [Fact]
    public void WithDiagnostics_NullInstance_ThrowsArgumentNullException()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act & Assert
        services
            .Invoking(s => s.AddPipeline<TestContext, TestResponse>(pipeline =>
            {
                pipeline
                    .AddStep<Step1>()
                    .AddStep<FinalStep>()
                    .WithDiagnostics(null!)
                    ;
            }))
            .Should()
            .Throw<ArgumentNullException>();
    }

    [Fact]
    public async Task WithDiagnostics_NotRegistered_UsesNullDiagnostics()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddPipeline<TestContext, TestResponse>(pipeline =>
        {
            pipeline
                .AddStep<Step1>()
                .AddStep<FinalStep>()
                ;
        });

        var provider = services.BuildServiceProvider();

        // Act
        var diagnostics = provider.GetRequiredService<IPipelineDiagnostics>();

        // Assert
        diagnostics
            .Should()
            .BeOfType<NullPipelineDiagnostics>();
    }

    [Fact]
    public async Task WithDiagnostics_RecordsDurationAndStepOrder()
    {
        // Arrange
        var diagnostics = new TestDiagnostics();
        var services = new ServiceCollection();
        services.AddPipeline<TestContext, TestResponse>(pipeline =>
        {
            pipeline
                .AddStep<Step1>()
                .AddStep<Step2>()
                .AddStep<FinalStep>()
                .WithDiagnostics(diagnostics)
                ;
        });

        var provider = services.BuildServiceProvider();
        var pipeline = provider.GetRequiredService<IPipeline<TestContext, TestResponse>>();

        // Act
        await pipeline.ExecuteAsync(new TestContext());

        // Assert
        var eventList = diagnostics.Events;
        var step0StartIdx = eventList.FindIndex(e => e.Contains("starting_0"));
        var step1StartIdx = eventList.FindIndex(e => e.Contains("starting_1"));
        var step2StartIdx = eventList.FindIndex(e => e.Contains("starting_2"));

        step0StartIdx
            .Should()
            .BeLessThan(step1StartIdx, "Step 0 should start before Step 1");

        step1StartIdx
            .Should()
            .BeLessThan(step2StartIdx, "Step 1 should start before Step 2");
    }

    [Fact]
    public async Task WithDiagnostics_ExecutesPipelineSuccessfully()
    {
        // Arrange
        var diagnostics = new TestDiagnostics();
        var services = new ServiceCollection();
        services.AddPipeline<TestContext, TestResponse>(pipeline =>
        {
            pipeline
                .AddStep<Step1>()
                .AddStep<Step2>()
                .AddStep<FinalStep>()
                .WithDiagnostics(diagnostics)
                ;
        });

        var provider = services.BuildServiceProvider();
        var pipeline = provider.GetRequiredService<IPipeline<TestContext, TestResponse>>();
        var context = new TestContext { Value = 5 };

        // Act
        var result = await pipeline.ExecuteAsync(context);

        // Assert
        result
            .Should()
            .NotBeNull()
            .And.BeOfType<TestResponse>();

        result.Result
            .Should()
            .Be(35, "5 + 10 (Step1) + 20 (Step2) = 35");

        diagnostics.Events
            .Should()
            .HaveCount(7, "3 steps × 2 events (starting + completed) + 1 pipeline event + null response event")
            .And.StartWith("starting_0_Step1")
            .And.EndWith("pipeline_completed_True");
    }

    [Fact]
    public async Task WithDiagnostics_MultipleInstances_AreIndependent()
    {
        // Arrange
        var diagnostics1 = new TestDiagnostics();
        var diagnostics2 = new TestDiagnostics();

        var services = new ServiceCollection();
        services.AddPipeline<TestContext, TestResponse>(pipeline =>
        {
            pipeline
                .AddStep<Step1>()
                .AddStep<FinalStep>()
                .WithDiagnostics(diagnostics1)
                ;
        });

        var provider = services.BuildServiceProvider();
        var pipeline1 = provider.GetRequiredService<IPipeline<TestContext, TestResponse>>();

        // Act
        await pipeline1.ExecuteAsync(new TestContext());

        // Assert
        diagnostics1.Events
            .Should()
            .NotBeEmpty();

        diagnostics2.Events
            .Should()
            .BeEmpty("diagnostics2 was never registered or used");
    }

    // Failing step for testing exception handling
    private class FailingStep : IPipelineStep<TestContext, TestResponse>
    {
        public Task<TestResponse> RunAsync(
            TestContext context,
            PipeDelegate<TestContext, TestResponse> next,
            CancellationToken ct = default)
        {
            throw new InvalidOperationException("Step failed intentionally");
        }
    }
}