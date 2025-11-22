using System;
using Microsoft.Extensions.DependencyInjection;

namespace EasyPipe.Extensions.MicrosoftDependencyInjection.V2;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds a pipeline to the service collection.
    /// </summary>
    /// <typeparam name="TContext">The context type passed through pipeline steps</typeparam>
    /// <typeparam name="TResult">The result type returned by the pipeline</typeparam>
    /// <example>
    /// <code>
    /// services
    ///     .AddPipeline&lt;RequestContext, Response&gt;()
    ///     .AddStep&lt;ValidationStep&gt;()
    ///     .AddStep&lt;ProcessingStep&gt;()
    ///     .Build();
    /// 
    /// // Later: inject IPipeline&lt;RequestContext, Response&gt;
    /// </code>
    /// </example>
    public static EasyPipe.V2.PipelineBuilder<TContext, TResult> AddPipeline<TContext, TResult>(this IServiceCollection services)
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        return new EasyPipe.V2.PipelineBuilder<TContext, TResult>(services);
    }
}