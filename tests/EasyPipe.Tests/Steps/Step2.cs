using EasyPipe.Abstractions;

namespace EasyPipe.Tests.Steps;

public class Step2 : IPipelineStep<TestContext, TestResult>
{
    public async Task<TestResult> RunAsync(
        TestContext context,
        PipeDelegate<TestContext, TestResult> next,
        CancellationToken ct = default)
    {
        context.ExecutedSteps.Add("Step2");
        return await next(context, ct);
    }
}