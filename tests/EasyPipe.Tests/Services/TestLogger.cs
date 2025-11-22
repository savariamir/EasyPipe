namespace EasyPipe.Tests.Services;

public class TestLogger : ITestLogger
{
    private readonly List<string> _logs;

    public TestLogger(List<string> logs)
    {
        _logs = logs;
    }

    public void Log(string message) => _logs.Add(message);
}