using EasyPipe.Abstractions;

namespace EasyPipe.Tests.Steps;

public class SlowStep : IPipelineStep<TestContext, TestResult>
{
    public async Task<TestResult> RunAsync(
        TestContext context,
        PipeDelegate<TestContext, TestResult> next,
        CancellationToken ct = default)
    {
        await Task.Delay(1000, ct);
        return await next(context, ct);
    }
}