using EasyPipe.Extensions.MicrosoftDependencyInjection.V2;
using EasyPipe.Tests.Steps;
using EasyPipe.V2;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace EasyPipe.Tests;

public class PipelineBuilderTests : IDisposable
{
    private readonly PipelineTestFixture _fixture;

    public PipelineBuilderTests()
    {
        _fixture = new PipelineTestFixture();
    }

    public void Dispose()
    {
        (_fixture.Provider as ServiceProvider)?.Dispose();
    }

    /// <summary>
    /// TEST: Builder should successfully add a single step
    /// </summary>
    [Fact]
    public void AddStep_WithValidStep_ShouldAddStepType()
    {
        // Arrange & Act
        var builder = _fixture.Services.AddPipeline<TestContext, TestResult>();
        builder.AddStep<FirstStep>().Build();

        // Assert
        var compiled = _fixture.Services
            .BuildServiceProvider()
            .GetRequiredService<CompiledPipeline<TestContext, TestResult>>();

        compiled.Steps.Should().HaveCount(1);
        compiled.Steps[0].Should().Be(typeof(FirstStep));
    }

    /// <summary>
    /// TEST: Builder should add multiple steps in order
    /// </summary>
    [Fact]
    public void AddStep_WithMultipleSteps_ShouldMaintainOrder()
    {
        // Arrange & Act
        var builder = _fixture.Services.AddPipeline<TestContext, TestResult>()
            .AddStep<FirstStep>()
            .AddStep<SecondStep>()
            .AddStep<ThirdStep>().Build();

        // Assert
        var compiled = _fixture.Services
            .BuildServiceProvider()
            .GetRequiredService<CompiledPipeline<TestContext, TestResult>>();

        compiled.Steps.Should().HaveCount(3);
        compiled.Steps[0].Should().Be(typeof(FirstStep));
        compiled.Steps[1].Should().Be(typeof(SecondStep));
        compiled.Steps[2].Should().Be(typeof(ThirdStep));
    }

    /// <summary>
    /// TEST: Builder.Build() should return IServiceCollection for chaining
    /// </summary>
    [Fact]
    public void Build_ShouldReturnServiceCollection()
    {
        // Arrange
        var builder = _fixture.Services.AddPipeline<TestContext, TestResult>();
        builder.AddStep<FirstStep>();

        // Act
        var result = builder.Build();

        // Assert
        result.Should().BeEquivalentTo(_fixture.Services);
    }

    /// <summary>
    /// TEST: Build should throw when no steps are registered
    /// </summary>
    [Fact]
    public void Build_WithNoSteps_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var builder = _fixture.Services.AddPipeline<TestContext, TestResult>();

        // Act & Assert
        builder.Invoking(b => b.Build())
            .Should()
            .Throw<InvalidOperationException>()
            .WithMessage("*must have at least one step*");
    }

    /// <summary>
    /// TEST: Build should register steps as scoped
    /// </summary>
    [Fact]
    public void Build_ShouldRegisterStepsAsScoped()
    {
        // Arrange
        var builder = _fixture.Services.AddPipeline<TestContext, TestResult>();
        builder.AddStep<FirstStep>();
        builder.Build();

        // Act
        _fixture.BuildServiceProvider();

        // Assert
        using var scope1 = _fixture.Provider.CreateScope();
        using var scope2 = _fixture.Provider.CreateScope();

        var instance1 = scope1.ServiceProvider.GetRequiredService<FirstStep>();
        var instance2 = scope2.ServiceProvider.GetRequiredService<FirstStep>();

        instance1.Should().NotBeSameAs(instance2); // Different scopes = different instances
    }

    /// <summary>
    /// TEST: CompiledPipeline should be singleton
    /// </summary>
    [Fact]
    public void Build_ShouldRegisterCompiledPipelineAsSingleton()
    {
        // Arrange
        var builder = _fixture.Services.AddPipeline<TestContext, TestResult>();
        builder.AddStep<FirstStep>();
        builder.Build();
        _fixture.BuildServiceProvider();

        // Act
        var compiled1 = _fixture.Provider.GetRequiredService<CompiledPipeline<TestContext, TestResult>>();
        var compiled2 = _fixture.Provider.GetRequiredService<CompiledPipeline<TestContext, TestResult>>();

        // Assert
        compiled1.Should().BeSameAs(compiled2);
    }

    /// <summary>
    /// TEST: Build should register IPipeline interface
    /// </summary>
    [Fact]
    public void Build_ShouldRegisterIPipelineInterface()
    {
        // Arrange
        var builder = _fixture.Services.AddPipeline<TestContext, TestResult>();
        builder.AddStep<FirstStep>();
        builder.Build();
        _fixture.BuildServiceProvider();

        // Act
        var pipeline = _fixture.Provider.GetRequiredService<IPipeline<TestContext, TestResult>>();

        // Assert
        pipeline.Should().NotBeNull();
        pipeline.Should().BeOfType<Pipeline<TestContext, TestResult>>();
    }
}