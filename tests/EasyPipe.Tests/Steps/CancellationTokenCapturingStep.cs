using EasyPipe.Abstractions;

namespace EasyPipe.Tests.Steps;

public class CancellationTokenCapturingStep : IPipelineStep<TestContext, TestResult>
{
    public async Task<TestResult> RunAsync(
        TestContext context,
        PipeDelegate<TestContext, TestResult> next,
        CancellationToken ct = default)
    {
        context.UsedCancellationToken = ct;
        return await next(context, ct);
    }
}