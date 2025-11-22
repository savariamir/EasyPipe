using EasyPipe.Tests.Services;
using EasyPipe.V2;

namespace EasyPipe.Tests.Steps;

public class VoidStep2 : IPipelineStep<TestContext, Unit>
{
    private readonly CallTracker _tracker;

    public VoidStep2(CallTracker tracker) => _tracker = tracker;

    public async Task<Unit> RunAsync(
        TestContext context,
        PipeDelegate<TestContext, Unit> next,
        CancellationToken ct = default)
    {
        _tracker.RecordCall("VoidStep2");
        return await next(context, ct);
    }
}