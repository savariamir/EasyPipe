using EasyPipe.Tests.Services;
using EasyPipe.V2;

namespace EasyPipe.Tests.Steps;

public class VoidStep1 : IPipelineStep<TestContext, Unit>
{
    private readonly CallTracker _tracker;

    public VoidStep1(CallTracker tracker) => _tracker = tracker;

    public async Task<Unit> RunAsync(
        TestContext context,
        PipeDelegate<TestContext, Unit> next,
        CancellationToken ct = default)
    {
        _tracker.RecordCall("VoidStep1");
        return await next(context, ct);
    }
}