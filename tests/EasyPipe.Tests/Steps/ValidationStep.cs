using EasyPipe.Tests.Services;
using EasyPipe.V2;

namespace EasyPipe.Tests.Steps;

public class ValidationStep : IPipelineStep<TestContext, TestResult>
{
    private readonly ITestLogger _logger;

    public ValidationStep(ITestLogger logger) => _logger = logger;

    public async Task<TestResult> RunAsync(
        TestContext context,
        PipeDelegate<TestContext, TestResult> next,
        CancellationToken ct = default)
    {
        _logger.Log("Validated");
        return await next(context, ct);
    }
}