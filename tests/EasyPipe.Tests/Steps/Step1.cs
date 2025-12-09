using EasyPipe.Abstractions;

namespace EasyPipe.Tests.Steps;

public class Step1 : IPipelineStep<TestContext, TestResult>
{
    public async Task<TestResult> RunAsync(
        TestContext context,
        PipeDelegate<TestContext, TestResult> next,
        CancellationToken ct = default)
    {
        context.ExecutedSteps.Add("Step1");
        return await next(context, ct);
    }
}