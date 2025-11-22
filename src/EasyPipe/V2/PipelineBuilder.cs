using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace EasyPipe.V2;

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
///     .Build();
/// </code>
/// </summary>
public class PipelineBuilder<TContext, TResult>(IServiceCollection services)
{
    private readonly List<Type> _stepTypes = [];

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
    /// Builds the pipeline and registers it in the DI container.
    /// </summary>
    /// <returns>The service collection for chaining</returns>
    /// <exception cref="InvalidOperationException">If no steps are registered</exception>
    public IServiceCollection Build()
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

            services.AddScoped(stepType);
        }

        services.AddSingleton(new CompiledPipeline<TContext, TResult>(_stepTypes.ToArray()));
        
        services.TryAddSingleton<IPipelineDiagnostics, NullPipelineDiagnostics>();
        
        services.AddScoped<IPipeline<TContext, TResult>, Pipeline<TContext, TResult>>();

        return services;
    }
}