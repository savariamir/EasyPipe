using EasyPipe.V2;

namespace EasyPipe.Tests.Steps;

public class AsyncThrowingStep : IPipelineStep<TestContext, TestResult>
{
    public async Task<TestResult> RunAsync(
        TestContext context,
        PipeDelegate<TestContext, TestResult> next,
        CancellationToken ct = default)
    {
        await Task.Delay(10, ct);
        throw new InvalidOperationException("Async error");
    }
}