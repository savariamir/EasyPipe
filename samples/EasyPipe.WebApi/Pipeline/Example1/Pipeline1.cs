using EasyPipe.Abstractions;

namespace EasyPipe.WebApi.Pipeline.Example1
{
    public class Pipeline1 : IPipelineStep<PipelineContext, PipelineResponse>
    {
        public async Task<PipelineResponse> RunAsync(
            PipelineContext context,
            PipeDelegate<PipelineContext,PipelineResponse> next,
            CancellationToken ct = default)
        {
            context.Count++;

            var result = await next(context, ct);

            result.BackwardCount++;

            return result;
        }
    }
}