using EasyPipe.Abstractions;
using EasyPipe.Tests.Services;

namespace EasyPipe.Tests.Steps;

public class VoidShortCircuitStep : IPipelineStep<TestContext, Unit>
{
    private readonly ITestLogger _logger;

    public VoidShortCircuitStep(ITestLogger logger) => _logger = logger;

    public Task<Unit> RunAsync(
        TestContext context,
        PipeDelegate<TestContext, Unit> next,
        CancellationToken ct = default)
    {
        _logger.Log("VoidShortCircuit");
        return Task.FromResult(Unit.Value); // Don't call next
    }
}