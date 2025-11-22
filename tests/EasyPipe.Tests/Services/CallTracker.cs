namespace EasyPipe.Tests.Services;

public class CallTracker
{
    private readonly List<string> _calls;

    public CallTracker(List<string> calls)
    {
        _calls = calls;
    }

    public void RecordCall(string stepName) => _calls.Add(stepName);
}