// File: src/EasyPipe.Extensions.DependencyInjection/PipelineBuilder.cs

using EasyPipe.Abstractions;
using EasyPipe.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EasyPipe.Extensions.DependencyInjection;

/// <summary>
/// Fluent builder for configuring pipelines with dependency injection.
/// 
/// Example:
/// <code>
/// services
///     .AddPipeline&lt;RequestContext, Response&gt;()
///     .AddStep&lt;ValidationStep&gt;()
///     .AddStep&lt;ProcessingStep&gt;()
///     .AddStep&lt;ResultStep&gt;()
///     .WithDiagnostics&lt;PerformanceMonitoringDiagnostics&gt;()
///     .Build();
/// </code>
/// </summary>
public class PipelineBuilder<TContext, TResult>
{
    private readonly List<Type> _stepTypes;
    private readonly IServiceCollection _services;
    private Type? _diagnosticsType;

    public PipelineBuilder(IServiceCollection services)
    {
        _services = services;
        _stepTypes = new List<Type>();
        _diagnosticsType = null;
    }

    /// <summary>
    /// Registers a step in the pipeline.
    /// Steps are executed in registration order.
    /// </summary>
    /// <typeparam name="TStep">The step implementation type</typeparam>
    /// <returns>This builder for fluent chaining</returns>
    /// <exception cref="InvalidOperationException">If step type doesn't implement IPipelineStep</exception>
    public PipelineBuilder<TContext, TResult> AddStep<TStep>() where TStep : class, IPipelineStep<TContext, TResult>
    {
        _stepTypes.Add(typeof(TStep));
        return this;
    }

    /// <summary>
    /// Registers custom diagnostics for pipeline execution monitoring.
    /// </summary>
    /// <typeparam name="TDiagnostics">The diagnostics implementation type</typeparam>
    /// <returns>This builder for fluent chaining</returns>
    /// <exception cref="InvalidOperationException">If diagnostics type doesn't implement IPipelineDiagnostics</exception>
    public PipelineBuilder<TContext, TResult> WithDiagnostics<TDiagnostics>() where TDiagnostics : class, IPipelineDiagnostics
    {
        if (!typeof(IPipelineDiagnostics).IsAssignableFrom(typeof(TDiagnostics)))
        {
            throw new InvalidOperationException(
                $"Diagnostics type '{typeof(TDiagnostics).Name}' must implement IPipelineDiagnostics");
        }

        _diagnosticsType = typeof(TDiagnostics);
        return this;
    }

    /// <summary>
    /// Registers a diagnostics instance directly.
    /// Useful for simple cases where the diagnostics doesn't need dependency injection.
    /// </summary>
    /// <param name="diagnostics">The diagnostics instance</param>
    /// <returns>This builder for fluent chaining</returns>
    public PipelineBuilder<TContext, TResult> WithDiagnostics(IPipelineDiagnostics diagnostics)
    {
        ArgumentNullException.ThrowIfNull(diagnostics);
        _services.AddSingleton(diagnostics);
        return this;
    }

    /// <summary>
    /// Builds the pipeline and registers it in the DI container.
    /// </summary>
    /// <returns>The service collection for chaining</returns>
    /// <exception cref="InvalidOperationException">If no steps are registered</exception>
    internal IServiceCollection Build()
    {
        if (_stepTypes.Count == 0)
        {
            throw new InvalidOperationException(
                "Pipeline must have at least one step. Use AddStep<TStep>() to add steps.");
        }

        foreach (var stepType in _stepTypes)
        {
            if (!typeof(IPipelineStep<TContext, TResult>).IsAssignableFrom(stepType))
            {
                throw new InvalidOperationException(
                    $"Step type '{stepType.Name}' must implement IPipelineStep<{typeof(TContext).Name}, {typeof(TResult).Name}>");
            }

            _services.AddScoped(stepType);
        }

        _services.AddSingleton(new CompiledPipeline<TContext, TResult>(_stepTypes.ToArray()));

        if (_diagnosticsType != null)
        {
            _services.AddScoped(typeof(IPipelineDiagnostics), _diagnosticsType);
        }
        else
        {
            _services.TryAddSingleton<IPipelineDiagnostics, NullPipelineDiagnostics>();
        }

        _services.AddScoped<IPipeline<TContext, TResult>, Pipeline<TContext, TResult>>();

        return _services;
    }
}