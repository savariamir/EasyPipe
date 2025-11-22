using EasyPipe.V2;

namespace EasyPipe.Tests.Steps;

public class ShortCircuitStep : IPipelineStep<TestContext, TestResult>
{
    public async Task<TestResult> RunAsync(
        TestContext context,
        PipeDelegate<TestContext, TestResult> next,
        CancellationToken ct = default)
    {
        context.ExecutedSteps.Add("ShortCircuit");
        // Don't call next - short circuit the pipeline
        return new TestResult { ResultValue = "short_circuit_result", Success = true };
    }
}