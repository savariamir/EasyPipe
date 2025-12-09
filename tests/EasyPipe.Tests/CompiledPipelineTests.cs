using EasyPipe.Internal;
using EasyPipe.Tests.Steps;
using FluentAssertions;

namespace EasyPipe.Tests;

public class CompiledPipelineTests
{
    [Fact]
    public void Constructor_WithEmptySteps_ShouldThrowArgumentException()
    {
        // Arrange & Act
        Action act = () => new CompiledPipeline<TestContext, TestResult>([]);

        // Assert
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("*must have at least one step*");
    }


    [Fact]
    public void Constructor_WithNullSteps_ShouldThrowArgumentException()
    {
        // Arrange & Act
        Action act = () => new CompiledPipeline<TestContext, TestResult>(null!);

        // Assert
        act.Should()
            .Throw<ArgumentException>()
            .WithMessage("*must have at least one step*");
    }

    [Fact]
    public void Constructor_WithDuplicateSteps_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var stepType = typeof(Step1);
        var steps = new[] { stepType, stepType }; // Same step registered twice

        // Act
        Action act = () => new CompiledPipeline<TestContext, TestResult>(steps);

        // Assert
        act.Should()
            .Throw<InvalidOperationException>()
            .WithMessage("*Duplicate steps detected*");
    }


    [Fact]
    public void Constructor_ShouldStoreSteps()
    {
        // Arrange
        var steps = new[] { typeof(Step1), typeof(Step2) };

        // Act
        var compiled = new CompiledPipeline<TestContext, TestResult>(steps);

        // Assert
        compiled.Steps.Should().Equal(steps);
    }
}