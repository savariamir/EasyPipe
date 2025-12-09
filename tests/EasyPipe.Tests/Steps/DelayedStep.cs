using EasyPipe.Abstractions;

namespace EasyPipe.Tests.Steps;

public class DelayedStep : IPipelineStep<TestContext, TestResult>
{
    public async Task<TestResult> RunAsync(
        TestContext context,
        PipeDelegate<TestContext, TestResult> next,
        CancellationToken ct = default)
    {
        await Task.Delay(100, ct);
        return await next(context, ct);
    }
}