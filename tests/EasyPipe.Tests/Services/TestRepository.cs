namespace EasyPipe.Tests.Services;

public class TestRepository : ITestRepository
{
    public async Task<object> GetAsync(string id)
    {
        await Task.Delay(10);
        return new { id, data = "test" };
    }
}