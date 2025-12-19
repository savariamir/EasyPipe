using EasyPipe.Abstractions;

namespace EasyPipe.Internal;

/// <summary>
/// No-op diagnostics for cases where monitoring is not needed
/// </summary>
internal class NullPipelineDiagnostics : IPipelineDiagnostics
{
    public void OnStepStarting(Type stepType, int stepIndex) { }
    public void OnStepCompleted(Type stepType, int stepIndex, TimeSpan duration) { }
    public void OnStepFailed(Type stepType, int stepIndex, Exception exception, TimeSpan duration) { }
    public void OnPipelineCompleted(TimeSpan duration, bool success) { }
}