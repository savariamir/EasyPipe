using System.Threading;
using System.Threading.Tasks;

namespace EasyPipe.V2;

/// <summary>
/// Represents a single step in a pipeline that can process context and call the next step.
/// 
/// Execution Flow:
/// - Steps are executed in registration order
/// - Each step receives the context, a delegate to call the next step, and a cancellation token
/// - Steps MUST call 'next' to continue the pipeline or return early to short-circuit
/// - Steps MUST respect the cancellation token
/// 
/// Example:
/// <code>
/// public class LoggingStep : IPipelineStep&lt;RequestContext, Response&gt;
/// {
///     public async Task&lt;Response&gt; RunAsync(RequestContext context, PipeDelegate&lt;RequestContext, Response&gt; next, CancellationToken ct)
///     {
///         Console.WriteLine($"Processing: {context.Request}");
///         try
///         {
///             var result = await next(context, ct);
///             Console.WriteLine("Success");
///             return result;
///         }
///         catch (Exception ex)
///         {
///             Console.WriteLine($"Error: {ex.Message}");
///             throw;
///         }
///     }
/// }
/// </code>
/// </summary>
public interface IPipelineStep<TContext, TResult>
{
    /// <summary>
    /// Executes the step logic.
    /// </summary>
    /// <param name="context">The pipeline context passed through all steps</param>
    /// <param name="next">Delegate to invoke the next step in the pipeline</param>
    /// <param name="ct">Cancellation token - steps should respect this for long-running operations</param>
    /// <returns>The pipeline result</returns>
    Task<TResult> RunAsync(TContext context, PipeDelegate<TContext, TResult> next, CancellationToken ct = default);
}