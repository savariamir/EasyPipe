using EasyPipe.Tests.Services;
using EasyPipe.V2;

namespace EasyPipe.Tests.Steps;

public class LoggingStep : IPipelineStep<TestContext, TestResult>
{
    private readonly ITestLogger _logger;

    public LoggingStep(ITestLogger logger) => _logger = logger;

    public async Task<TestResult> RunAsync(
        TestContext context,
        PipeDelegate<TestContext, TestResult> next,
        CancellationToken ct = default)
    {
        _logger.Log("Started");
        return await next(context, ct);
    }
}