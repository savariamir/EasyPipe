using Microsoft.Extensions.DependencyInjection;

namespace EasyPipe.Tests;

/// <summary>
/// Base fixture for pipeline tests
/// </summary>
public class PipelineTestFixture
{
    public IServiceCollection Services { get; }
    public IServiceProvider Provider { get; private set; } = null!;

    public PipelineTestFixture()
    {
        Services = new ServiceCollection();
    }

    public void BuildServiceProvider()
    {
        Provider = Services.BuildServiceProvider();
    }
}