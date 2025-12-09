using EasyPipe.Abstractions;
using EasyPipe.WebApi.Pipeline.Example2;

public class InMemoryPipelineStep : IPipelineStep<ArticleRequest, ArticleResponse>
{
    public Task<ArticleResponse> RunAsync(ArticleRequest context,
        PipeDelegate<ArticleRequest,
            ArticleResponse> next,
        CancellationToken ct = default)
    {
        return next(context, ct);
    }
}