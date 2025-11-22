using EasyPipe.Tests.Services;
using EasyPipe.V2;

namespace EasyPipe.Tests.Steps;

public class ProcessingStep : IPipelineStep<TestContext, TestResult>
{
    private readonly ITestLogger _logger;
    private readonly ITestRepository _repository;

    public ProcessingStep(ITestLogger logger, ITestRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<TestResult> RunAsync(
        TestContext context,
        PipeDelegate<TestContext, TestResult> next,
        CancellationToken ct = default)
    {
        await _repository.GetAsync(context.Value);
        _logger.Log("Processed");
        return await next(context, ct);
    }
}