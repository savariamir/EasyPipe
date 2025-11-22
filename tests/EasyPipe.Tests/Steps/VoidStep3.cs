using EasyPipe.Tests.Services;
using EasyPipe.V2;

namespace EasyPipe.Tests.Steps;

public class VoidStep3 : IPipelineStep<EventContext, Unit>
{
    private readonly CallTracker _tracker;

    public VoidStep3(CallTracker tracker) => _tracker = tracker;

    public async Task<Unit> RunAsync(
        EventContext context,
        PipeDelegate<EventContext, Unit> next,
        CancellationToken ct = default)
    {
        _tracker.RecordCall("VoidStep1");
        return await next(context, ct);
    }
}