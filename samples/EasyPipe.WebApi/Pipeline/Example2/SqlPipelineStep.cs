using EasyPipe.Abstractions;

namespace EasyPipe.WebApi.Pipeline.Example2;

public class SqlPipelineStep : IPipelineStep<ArticleRequest, ArticleResponse>
{
    public Task<ArticleResponse> RunAsync(ArticleRequest context, PipeDelegate<ArticleRequest, ArticleResponse> next, CancellationToken ct = default)
    {
        return Task.FromResult(new ArticleResponse
        {
            Title = "Title from SQL Database",
            Content = "This is content from SQL Database"
        });
    }
}