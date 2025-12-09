using EasyPipe.Abstractions;
using EasyPipe.Extensions.DependencyInjection;
using EasyPipe.WebApi.Pipeline.Example1;
using EasyPipe.WebApi.Pipeline.Example2;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add built-in OpenAPI support
builder.Services.AddOpenApi();

builder.Services.AddPipeline<PipelineContext, PipelineResponse>(pipeline =>
{
    pipeline
        .AddStep<Pipeline1>()
        .AddStep<Pipeline2>()
        .AddStep<Pipeline3>();
});

builder.Services.AddPipeline<ArticleRequest, ArticleResponse>(pipeline =>
{
    pipeline
        .AddStep<InMemoryPipelineStep>()
        .AddStep<RedisPipelineStep>()
        .AddStep<SqlPipelineStep>();
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/example1", async ([FromServices] IPipeline<PipelineContext, PipelineResponse> pipeline) =>
    {
        var result = await pipeline.ExecuteAsync(new PipelineContext());
        return result;
    })
    .WithName("example1");

app.MapGet("/example2", async ([FromServices] IPipeline<ArticleRequest, ArticleResponse> pipeline) =>
    {
        var result = await pipeline.ExecuteAsync(new ArticleRequest());
        return result;
    })
    .WithName("example2");

app.Run();