namespace EasyPipe.Tests.Services;

public class ServiceTracker: IDisposable
{
    private readonly Action _onDispose;

    public ServiceTracker(Action onDispose)
    {
        _onDispose = onDispose;
    }
    
    public void Dispose()
    {
        _onDispose();
    }
}