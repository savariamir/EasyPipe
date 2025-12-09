using EasyPipe.Abstractions;

namespace EasyPipe.Tests;

internal class PipelineDiagnostics: IPipelineDiagnostics
{
    public void OnStepStarting(Type stepType, int stepIndex)
    {
    }

    public void OnStepCompleted(Type stepType, int stepIndex, TimeSpan duration)
    {
    }

    public void OnStepFailed(Type stepType, int stepIndex, Exception exception, TimeSpan duration)
    {
    }

    public void OnPipelineCompleted(TimeSpan duration, bool success)
    {
    }
}