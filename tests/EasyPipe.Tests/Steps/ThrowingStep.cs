using EasyPipe.V2;

namespace EasyPipe.Tests.Steps;

public class ThrowingStep : IPipelineStep<TestContext, TestResult>
{
    public Task<TestResult> RunAsync(
        TestContext context,
        PipeDelegate<TestContext, TestResult> next,
        CancellationToken ct = default)
    {
        throw new InvalidOperationException("Step failed");
    }
}