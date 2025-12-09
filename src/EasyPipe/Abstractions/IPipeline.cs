using System.Threading;
using System.Threading.Tasks;

namespace EasyPipe.Abstractions;

/// <summary>
/// Pipeline interface for dependency injection
/// </summary>
public interface IPipeline<in TContext, TResult>
{
    Task<TResult> ExecuteAsync(TContext context, CancellationToken ct = default);
}