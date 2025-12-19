using EasyPipe.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace EasyPipe.Internal;

/// <summary>
/// Pipeline executor that manages step orchestration and dependency resolution.
/// </summary>
internal sealed class Pipeline<TContext, TResult>(
    IServiceProvider sp,
    CompiledPipeline<TContext, TResult> compiled,
    IPipelineDiagnostics diagnostics) : IPipeline<TContext, TResult>
{
    public async Task<TResult> ExecuteAsync(TContext context, CancellationToken ct = default)
    {
        var startTime = DateTime.UtcNow;

        var scope = sp.GetRequiredService<IServiceScopeFactory>().CreateScope();
        using (scope)
        {
            var provider = scope.ServiceProvider;

            try
            {
                var result = await ExecuteStepAsync(0, context, provider, compiled.Steps, ct);

                var duration = DateTime.UtcNow - startTime;
                diagnostics?.OnPipelineCompleted(duration, true);

                return result;
            }
            catch (Exception)
            {
                var duration = DateTime.UtcNow - startTime;
                diagnostics?.OnPipelineCompleted(duration, false);

                throw;
            }
        }
    }

    private async Task<TResult> ExecuteStepAsync(
        int stepIndex,
        TContext context,
        IServiceProvider provider,
        Type[] stepTypes,
        CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();

        // Terminal condition: no more steps
        if (stepIndex >= stepTypes.Length)
        {
            return default!;
        }

        var stepType = stepTypes[stepIndex];

        try
        {
            var stepWatch = System.Diagnostics.Stopwatch.StartNew();
            diagnostics?.OnStepStarting(stepType, stepIndex);

            var step = (IPipelineStep<TContext, TResult>)provider.GetRequiredService(stepType);

            Task<TResult> NextDelegate(TContext nextContext, CancellationToken nextCt) =>
                ExecuteStepAsync(stepIndex + 1, nextContext, provider, stepTypes, nextCt);

            var result = await step.RunAsync(context, NextDelegate, ct);

            stepWatch.Stop();
            diagnostics?.OnStepCompleted(stepType, stepIndex, stepWatch.Elapsed);

            return result;
        }
        catch (Exception ex)
        {
            diagnostics?.OnStepFailed(stepType, stepIndex, ex, System.Diagnostics.Stopwatch.StartNew().Elapsed);
            throw;
        }
    }
}