using EasyPipe.Abstractions;

namespace EasyPipe.Tests.Steps;

public class ResultStep : IPipelineStep<TestContext, TestResult>
{
    public async Task<TestResult> RunAsync(
        TestContext context,
        PipeDelegate<TestContext, TestResult> next,
        CancellationToken ct = default)
    {
        await next(context, ct);
        return new TestResult { Success = true, ResultValue = "completed" };
    }
}