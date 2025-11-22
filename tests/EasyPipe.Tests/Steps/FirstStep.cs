using EasyPipe.V2;

namespace EasyPipe.Tests.Steps;

public class FirstStep : IPipelineStep<TestContext, TestResult>
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