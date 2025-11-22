namespace EasyPipe.Tests.Services;

public interface ITestRepository
{
    Task<object> GetAsync(string id);
}