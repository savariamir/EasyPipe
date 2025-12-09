using EasyPipe.Abstractions;

namespace EasyPipe.Tests.Steps;

public class NullableContextStep : IPipelineStep<TestContext?, TestResult>
{
    public async Task<TestResult> RunAsync(
        TestContext? context,
        PipeDelegate<TestContext?, TestResult> next,
        CancellationToken ct = default)
    {
        return await next(context, ct);
    }
}