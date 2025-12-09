using EasyPipe.Abstractions;

namespace EasyPipe.Tests.Steps;

public class ShortCircuitStep : IPipelineStep<TestContext, TestResult>
{
    public Task<TestResult> RunAsync(
        TestContext context,
        PipeDelegate<TestContext, TestResult> next,
        CancellationToken ct = default)
    {
        context.ExecutedSteps.Add("ShortCircuit");
        return Task.FromResult(new TestResult { ResultValue = "short_circuit_result", Success = true });
    }
}