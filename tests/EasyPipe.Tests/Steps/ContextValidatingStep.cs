using EasyPipe.Abstractions;

namespace EasyPipe.Tests.Steps;

public class ContextValidatingStep : IPipelineStep<TestContext, TestResult>
{
    public async Task<TestResult> RunAsync(
        TestContext context,
        PipeDelegate<TestContext, TestResult> next,
        CancellationToken ct = default)
    {
        if (context.Value.Contains("modified"))
            return await next(context, ct);

        throw new InvalidOperationException("Context not modified");
    }
}