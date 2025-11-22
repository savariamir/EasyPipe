using System;
using System.Collections.Generic;

namespace EasyPipe.V2;

/// <summary>
/// Represents pre-compiled pipeline configuration with validation.
/// </summary>
public sealed class CompiledPipeline<TContext, TResult>
{
    public Type[] Steps { get; }
    
    public CompiledPipeline(Type[] steps)
    {
        if (steps == null || steps.Length == 0)
        {
            throw new ArgumentException("Pipeline must have at least one step", nameof(steps));
        }

        var stepTypeSet = new HashSet<Type>(steps);
        if (stepTypeSet.Count != steps.Length)
        {
            throw new InvalidOperationException(
                "Duplicate steps detected in pipeline configuration. " +
                "This may cause infinite loops or unexpected behavior.");
        }

        Steps = steps;
    }
}