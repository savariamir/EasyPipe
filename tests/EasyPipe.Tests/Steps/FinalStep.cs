using EasyPipe.Abstractions;

namespace EasyPipe.Tests.Steps;

public class FinalStep : IPipelineStep<TestContext, TestResult>
{
    public Task<TestResult> RunAsync(
        TestContext context,
        PipeDelegate<TestContext, TestResult> next,
        CancellationToken ct = default)
    {
        return Task.FromResult(new TestResult { ResultCounter = context.Counter });
    }
}