using System.Diagnostics;
using EasyPipe.Abstractions;
using EasyPipe.Extensions.DependencyInjection;
using EasyPipe.Tests.Steps;
using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace EasyPipe.Tests;

public class PipelinePerformanceTests : IDisposable
{
    private readonly PipelineTestFixture _fixture;

    public PipelinePerformanceTests()
    {
        _fixture = new PipelineTestFixture();
    }

    public void Dispose()
    {
        (_fixture.Provider as ServiceProvider)?.Dispose();
    }

    [Fact]
    public async Task ExecuteAsync_ShouldCompleteInReasonableTime()
    {
        // Arrange
        _fixture.Services.AddPipeline<TestContext, TestResult>(pipeline => pipeline
            .AddStep<Step1>()
            .AddStep<Step2>()
            .AddStep<Step3>());
        _fixture.BuildServiceProvider();
        var pipeline = _fixture.Provider.GetRequiredService<IPipeline<TestContext, TestResult>>();

        var sw = Stopwatch.StartNew();

        // Act
        const int iterations = 100;
        for (var i = 0; i < iterations; i++)
        {
            await pipeline.ExecuteAsync(new TestContext());
        }

        sw.Stop();

        // Assert
        var avgTime = sw.ElapsedMilliseconds / (double)iterations;
        avgTime.Should().BeLessThan(10); // Average execution should be < 10ms
    }


    [Fact]
    public async Task ExecuteAsync_WithHighConcurrency_ShouldHandleLoad()
    {
        // Arrange
        _fixture.Services.AddPipeline<TestContext, TestResult>(pipeline =>
        {
            pipeline
                .AddStep<ResultStep>();
        });

        _fixture.BuildServiceProvider();
        var pipeline = _fixture.Provider.GetRequiredService<IPipeline<TestContext, TestResult>>();

        // Act
        var tasks = Enumerable.Range(0, 100)
            .Select(_ => pipeline.ExecuteAsync(new TestContext()))
            .ToList();

        var results = await Task.WhenAll(tasks);

        // Assert
        results.Should().HaveCount(100);
        results.Should().AllSatisfy(r => r.Should().NotBeNull());
    }
}