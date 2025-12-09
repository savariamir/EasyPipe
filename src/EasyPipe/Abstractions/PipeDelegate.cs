using System.Threading;
using System.Threading.Tasks;

namespace EasyPipe.Abstractions;

/// <summary>
/// Represents the continuation delegate for pipeline execution.
/// This is passed to each step to allow calling the next step in the chain.
/// 
/// Example: await next(modifiedContext, cancellationToken)
/// </summary>
public delegate Task<TResult> PipeDelegate<in TContext, TResult>(TContext context, CancellationToken ct = default);