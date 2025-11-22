namespace EasyPipe.Tests.Services;

public class TestDbContext : IDisposable
{
    private readonly Action _onDispose;

    public TestDbContext(Action onDispose)
    {
        _onDispose = onDispose;
    }

    public void Dispose()
    {
        _onDispose();
    }
}