using System;
using Microsoft.Extensions.DependencyInjection;

namespace EasyPipe.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds a pipeline to the service collection.
    /// </summary>
    /// <typeparam name="TContext">The context type passed through pipeline steps</typeparam>
    /// <typeparam name="TResult">The result type returned by the pipeline</typeparam>
    /// <example>
    /// <code>
    /// services.AddPipeline&lt;RequestContext, Response&gt;(pipeline =&gt;
    /// {
    ///         pipeline
    ///          .AddStep&lt;ValidationStep&gt;()
    ///          .AddStep&lt;ProcessingStep&gt;());
    /// });
    /// </code>
    /// </example>
    public static IServiceCollection AddPipeline<TContext, TResult>(
        this IServiceCollection services,
        Action<PipelineBuilder<TContext, TResult>> configure)
    {
        ArgumentNullException.ThrowIfNull(configure);
        ArgumentNullException.ThrowIfNull(services);

        var builder = new PipelineBuilder<TContext, TResult>(services);

        configure(builder);

        builder.Build();

        return services;
    }
}