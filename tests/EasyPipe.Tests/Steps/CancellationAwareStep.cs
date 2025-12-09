using EasyPipe.Abstractions;

namespace EasyPipe.Tests.Steps;

public class CancellationAwareStep : IPipelineStep<TestContext, TestResult>
{
    public Task<TestResult> RunAsync(
        TestContext context,
        PipeDelegate<TestContext, TestResult> next,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        return next(context, ct);
    }
}