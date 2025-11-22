using EasyPipe.V2;

namespace EasyPipe.Tests.Steps;

public class CompletedTaskStep : IPipelineStep<TestContext, TestResult>
{
    public Task<TestResult> RunAsync(
        TestContext context,
        PipeDelegate<TestContext, TestResult> next,
        CancellationToken ct = default)
    {
        return next(context, ct);
    }
}