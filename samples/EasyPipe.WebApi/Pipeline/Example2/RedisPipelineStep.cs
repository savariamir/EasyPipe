using EasyPipe.Abstractions;

namespace EasyPipe.WebApi.Pipeline.Example2;

public class RedisPipelineStep : IPipelineStep<ArticleRequest, ArticleResponse>
{
    public async Task<ArticleResponse> RunAsync(ArticleRequest context, PipeDelegate<ArticleRequest, ArticleResponse> next, CancellationToken ct = default)
    {
       return await next(context, ct);
    }
}