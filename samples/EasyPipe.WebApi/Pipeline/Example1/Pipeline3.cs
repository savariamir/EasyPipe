using EasyPipe.Abstractions;

namespace EasyPipe.WebApi.Pipeline.Example1
{
    public class Pipeline3 : IPipelineStep<PipelineContext, PipelineResponse>
    {
        public async Task<PipelineResponse> RunAsync(PipelineContext context, PipeDelegate<PipelineContext, PipelineResponse> next, CancellationToken ct = default)
        {
            context.Count++;

            PipelineResponse result = await next(context, ct) ?? new PipelineResponse
            {
                ForwardCount = context.Count
            };

            result.BackwardCount++;

            return result;
        }
    }
}