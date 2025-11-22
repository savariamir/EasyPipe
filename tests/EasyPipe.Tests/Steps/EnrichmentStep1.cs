using EasyPipe.V2;

namespace EasyPipe.Tests.Steps;

public class EnrichmentStep1 : IPipelineStep<TestContext, TestResult>
{
    public async Task<TestResult> RunAsync(
        TestContext context,
        PipeDelegate<TestContext, TestResult> next,
        CancellationToken ct = default)
    {
        context.Value += "_enriched1";
        return await next(context, ct);
    }
}