using System;

namespace EasyPipe.V2;

/// <summary>
/// No-op diagnostics for cases where monitoring is not needed
/// </summary>
public class NullPipelineDiagnostics : IPipelineDiagnostics
{
    public void OnStepStarting(Type stepType, int stepIndex) { }
    public void OnStepCompleted(Type stepType, int stepIndex, TimeSpan duration) { }
    public void OnStepFailed(Type stepType, int stepIndex, Exception exception, TimeSpan duration) { }
    public void OnPipelineCompleted(TimeSpan duration, bool success) { }
}