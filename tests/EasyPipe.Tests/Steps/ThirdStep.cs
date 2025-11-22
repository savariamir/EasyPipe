using EasyPipe.V2;

namespace EasyPipe.Tests.Steps;

public class ThirdStep : IPipelineStep<TestContext, TestResult>
{
    public async Task<TestResult> RunAsync(
        TestContext context,
        PipeDelegate<TestContext, TestResult> next,
        CancellationToken ct = default)
    {
        context.ExecutedSteps.Add("Step3");
        return await next(context, ct);
    }
}