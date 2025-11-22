using EasyPipe.V2;

namespace EasyPipe.Tests.Steps;

public class ContextModifyingStep : IPipelineStep<TestContext, TestResult>
{
    public async Task<TestResult> RunAsync(
        TestContext context,
        PipeDelegate<TestContext, TestResult> next,
        CancellationToken ct = default)
    {
        context.Value = "modified";
        return await next(context, ct);
    }
}