namespace EasyPipe.Abstractions;

/// <summary>
/// Provides diagnostics and observability for pipeline execution.
/// </summary>
public interface IPipelineDiagnostics
{
    /// <summary>Called before a step executes</summary>
    void OnStepStarting(Type stepType, int stepIndex);

    /// <summary>Called after a step completes successfully</summary>
    void OnStepCompleted(Type stepType, int stepIndex, TimeSpan duration);

    /// <summary>Called when a step throws an exception</summary>
    void OnStepFailed(Type stepType, int stepIndex, Exception exception, TimeSpan duration);

    /// <summary>Called when the entire pipeline completes</summary>
    void OnPipelineCompleted(TimeSpan duration, bool success);
}