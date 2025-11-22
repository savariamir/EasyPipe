using EasyPipe.Extensions.MicrosoftDependencyInjection.V2;
using EasyPipe.V2;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace EasyPipe.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddPipeline_ShouldReturnPipelineBuilder()
    {
        // Arrange
        var services = new ServiceCollection();

        // Act
        var builder = services.AddPipeline<TestContext, TestResult>();

        // Assert
        builder.Should().BeOfType<PipelineBuilder<TestContext, TestResult>>();
    }

    [Fact]
    public void AddPipeline_WithNullServices_ShouldThrowArgumentNullException()
    {
        // Arrange
        IServiceCollection? services = null;

        // Act & Assert
        Action act = () => services!.AddPipeline<TestContext, TestResult>();

        act.Should()
            .Throw<ArgumentNullException>();
    }
}