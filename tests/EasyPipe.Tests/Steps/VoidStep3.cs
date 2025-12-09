using EasyPipe.Abstractions;
using EasyPipe.Tests.Services;

namespace EasyPipe.Tests.Steps;

public class VoidStep3 : IPipelineStep<TestContext, Unit>
{
    private readonly ITestLogger _logger;

    public VoidStep3(ITestLogger logger) => _logger = logger;

    public async Task<Unit> RunAsync(
        TestContext context,
        PipeDelegate<TestContext, Unit> next,
        CancellationToken ct = default)
    {
        _logger.Log("VoidStep1");
        return await next(context, ct);
    }
}