using EasyPipe.Abstractions;
using EasyPipe.Extensions.DependencyInjection;
using EasyPipe.Internal;
using EasyPipe.Tests.Steps;
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


    [Fact]
    public void AddStep_WithValidStep_ShouldAddStepType()
    {
        // Arrange & Act
        _fixture.Services.AddPipeline<TestContext, TestResult>(pipeline =>
        {
            pipeline
                .AddStep<Step1>();
        });

        // Assert
        var compiled = _fixture.Services
            .BuildServiceProvider()
            .GetRequiredService<CompiledPipeline<TestContext, TestResult>>();

        compiled.Steps.Should().HaveCount(1);
        compiled.Steps[0].Should().Be(typeof(Step1));
    }

    [Fact]
    public void AddStep_WithMultipleSteps_ShouldMaintainOrder()
    {
        // Arrange & Act
        _fixture.Services.AddPipeline<TestContext, TestResult>(pipeline =>
        {
            pipeline
                .AddStep<Step1>()
                .AddStep<Step2>()
                .AddStep<Step3>();
        });

        // Assert
        var compiled = _fixture.Services
            .BuildServiceProvider()
            .GetRequiredService<CompiledPipeline<TestContext, TestResult>>();

        compiled.Steps.Should().HaveCount(3);
        compiled.Steps[0].Should().Be(typeof(Step1));
        compiled.Steps[1].Should().Be(typeof(Step2));
        compiled.Steps[2].Should().Be(typeof(Step3));
    }


    [Fact]
    public void Build_ShouldReturnServiceCollection()
    {
        // Arrange & Act
        var services = _fixture.Services.AddPipeline<TestContext, TestResult>(pipeline =>
        {
            pipeline.AddStep<Step1>();
        });

        // Assert
        services.Should().BeEquivalentTo(_fixture.Services);
    }

    [Fact]
    public void AddPipeline_WithNullServices_ShouldThrowArgumentNullException()
    {
        // Arrange
        IServiceCollection? services = null;

        // Act & Assert
        Action act = () => services!.AddPipeline<TestContext, TestResult>((_) => { });

        act.Should()
            .Throw<ArgumentNullException>();
    }


    [Fact]
    public void Build_WithNoSteps_ShouldThrowInvalidOperationException()
    {
        // Arrange
        Action act = () => _fixture.Services.AddPipeline<TestContext, TestResult>(_ => { });

        // Act & Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*must have at least one step*");
    }

    /// <summary>
    /// TEST: Build should register steps as scoped
    /// </summary>
    [Fact]
    public void Build_ShouldRegisterStepsAsScoped()
    {
        // Arrange
        _fixture.Services.AddPipeline<TestContext, TestResult>(pipeline =>
        {
            pipeline
                .AddStep<Step1>();
        });

        // Act
        _fixture.BuildServiceProvider();

        // Assert
        using var scope1 = _fixture.Provider.CreateScope();
        using var scope2 = _fixture.Provider.CreateScope();

        var instance1 = scope1.ServiceProvider.GetRequiredService<Step1>();
        var instance2 = scope2.ServiceProvider.GetRequiredService<Step1>();

        instance1.Should().NotBeSameAs(instance2);
    }


    [Fact]
    public void Build_ShouldRegisterCompiledPipelineAsSingleton()
    {
        // Arrange
        _fixture.Services.AddPipeline<TestContext, TestResult>(pipeline => pipeline
            .AddStep<Step1>());
        _fixture.BuildServiceProvider();

        // Act
        var compiled1 = _fixture.Provider.GetRequiredService<CompiledPipeline<TestContext, TestResult>>();
        var compiled2 = _fixture.Provider.GetRequiredService<CompiledPipeline<TestContext, TestResult>>();

        // Assert
        compiled1.Should().BeSameAs(compiled2);
    }


    [Fact]
    public void Build_ShouldRegisterIPipelineInterface()
    {
        // Arrange
        _fixture.Services.AddPipeline<TestContext, TestResult>(pipeline =>
        {
            pipeline
                .AddStep<Step1>();
        });
        _fixture.BuildServiceProvider();

        // Act
        var pipeline = _fixture.Provider.GetRequiredService<IPipeline<TestContext, TestResult>>();

        // Assert
        pipeline.Should().NotBeNull();
        pipeline.Should().BeOfType<Pipeline<TestContext, TestResult>>();
    }
}