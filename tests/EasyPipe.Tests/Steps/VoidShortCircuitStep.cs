using EasyPipe.Tests.Services;
using EasyPipe.V2;

namespace EasyPipe.Tests.Steps;

public class VoidShortCircuitStep : IPipelineStep<TestContext, Unit>
{
    private readonly CallTracker _tracker;

    public VoidShortCircuitStep(CallTracker tracker) => _tracker = tracker;

    public Task<Unit> RunAsync(
        TestContext context,
        PipeDelegate<TestContext, Unit> next,
        CancellationToken ct = default)
    {
        _tracker.RecordCall("VoidShortCircuit");
        return Task.FromResult(Unit.Value); // Don't call next
    }
}