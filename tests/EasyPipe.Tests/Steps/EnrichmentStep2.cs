using EasyPipe.V2;

namespace EasyPipe.Tests.Steps;

public class EnrichmentStep2 : IPipelineStep<TestContext, TestResult>
{
    public async Task<TestResult> RunAsync(
        TestContext context,
        PipeDelegate<TestContext, TestResult> next,
        CancellationToken ct = default)
    {
        context.Value += "_enriched2";
        return await next(context, ct);
    }
}