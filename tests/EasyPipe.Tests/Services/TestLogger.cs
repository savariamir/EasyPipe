namespace EasyPipe.Tests.Services;

public class TestLogger(List<string> logs) : ITestLogger
{
    public void Log(string message) => logs.Add(message);
}