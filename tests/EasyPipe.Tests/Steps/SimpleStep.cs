using EasyPipe.V2;

namespace EasyPipe.Tests.Steps;

// public class SimpleStep : IPipelineStep<TestContext, TestResult>
// {
//     public async Task<TestResult> RunAsync(
//         TestContext context,
//         PipeDelegate<TestContext, TestResult> next,
//         CancellationToken ct = default)
//     {
//         await next(context, ct);
//         return new TestResult { ResultValue = "simple_result", Success = true };
//     }
// }