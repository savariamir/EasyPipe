namespace EasyPipe.Internal;

internal sealed class CompiledPipeline<TContext, TResult>
{
    public Type[] Steps { get; }

    public CompiledPipeline(Type[] steps)
    {
        if (steps == null || steps.Length == 0)
        {
            throw new ArgumentException("Pipeline must have at least one step", nameof(steps));
        }

        if (typeof(TContext) == typeof(TResult))
        {
            throw new InvalidOperationException(
                "Pipeline context type and result type cannot be the same. " +
                "This may lead to ambiguous behavior.");
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