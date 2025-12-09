# EasyPipe

[![.NET 5 CI](https://github.com/savariamir/EasyPipe/actions/workflows/dotnet.yml/badge.svg)](https://github.com/savariamir/EasyPipe/actions/workflows/dotnet.yml)
[![NuGet](https://img.shields.io/nuget/vpre/EasyPipe.svg)](https://www.nuget.org/packages/EasyPipe)

A lightweight, dependency-free library for building composable pipelines in .NET. EasyPipe makes it simple to implement the middleware/pipeline pattern with a clean, fluent API.

## Features

- ✅ **No Dependencies** — Core library has zero external dependencies
- ✅ **No Reflection** — Blazing-fast execution with minimal overhead
- ✅ **Simple API** — Intuitive fluent interface for defining pipelines
- ✅ **.NET 9 & 10** — Targets modern .NET frameworks
- ✅ **Async First** — Full async/await support with cancellation tokens
- ✅ **Diagnostic Hooks** — Optional monitoring for step execution

## Installation

Install via NuGet:

```bash
dotnet add package EasyPipe
```

For dependency injection integration:

```bash
dotnet add package EasyPipe.Extensions.MicrosoftDependencyInjection
```

## Quick Start

### 1. Define Your Context and Result Types

```csharp
public class RequestContext
{
    public string Data { get; set; }
}

public class Response
{
    public string Result { get; set; }
}
```

### 2. Create Pipeline Steps

Implement `IPipelineStep<TContext, TResult>` for each processing step:

```csharp
public class ValidationStep : IPipelineStep<RequestContext, Response>
{
    public async Task<Response> RunAsync(
        RequestContext context,
        PipeDelegate<RequestContext, Response> next,
        CancellationToken ct = default)
    {
        // Validate input
        if (string.IsNullOrEmpty(context.Data))
        {
            return new Response { Result = "Invalid input" };
        }

        // Continue to next step
        return await next(context, ct);
    }
}

public class ProcessingStep : IPipelineStep<RequestContext, Response>
{
    public async Task<Response> RunAsync(
        RequestContext context,
        PipeDelegate<RequestContext, Response> next,
        CancellationToken ct = default)
    {
        // Process the data
        context.Data = context.Data.ToUpper();
        
        return await next(context, ct);
    }
}

public class ResultStep : IPipelineStep<RequestContext, Response>
{
    public Task<Response> RunAsync(
        RequestContext context,
        PipeDelegate<RequestContext, Response> next,
        CancellationToken ct = default)
    {
        // Terminal step - return the response
        return Task.FromResult(new Response 
        { 
            Result = context.Data 
        });
    }
}
```

### 3. Register the Pipeline

Register your pipeline in the dependency injection container:

```csharp
services.AddPipeline<RequestContext, Response>(pipeline =>
{
    pipeline
        .AddStep<ValidationStep>()
        .AddStep<ProcessingStep>()
        .AddStep<ResultStep>()
});
```

### 4. Use the Pipeline

Inject and execute the pipeline:

```csharp
[ApiController]
[Route("[controller]")]
public class DataController
{
    private readonly IPipeline<RequestContext, Response> _pipeline;

    public DataController(IPipeline<RequestContext, Response> pipeline)
    {
        _pipeline = pipeline;
    }

    [HttpPost]
    public async Task<ActionResult<Response>> Process([FromBody] RequestContext request)
    {
        var result = await _pipeline.ExecuteAsync(request);
        return Ok(result);
    }
}
```

## How It Works

### Execution Flow

Steps are executed in registration order. Each step receives:

- **context** — The current request/context object
- **next** — A delegate to invoke the next step in the pipeline
- **ct** — A cancellation token for graceful cancellation

Steps can:
- **Continue** — Call `await next(context, ct)` to proceed to the next step
- **Short-circuit** — Return early without calling `next` to skip remaining steps
- **Transform** — Modify the context before passing it forward or modify the response on the way back

### Example: Caching Pattern

```csharp
public class CacheStep : IPipelineStep<RequestContext, Response>
{
    private readonly IMemoryCache _cache;

    public CacheStep(IMemoryCache cache) => _cache = cache;

    public async Task<Response> RunAsync(
        RequestContext context,
        PipeDelegate<RequestContext, Response> next,
        CancellationToken ct = default)
    {
        // Check cache
        if (_cache.TryGetValue(context.Data, out Response cached))
        {
            return cached;
        }

        // Call next steps
        var result = await next(context, ct);

        // Store in cache
        _cache.Set(context.Data, result, TimeSpan.FromHours(1));

        return result;
    }
}
```

## Real-World Scenarios

### Multi-Storage Cache Lookup

When retrieving an article, check multiple storage systems (memory, Redis, database) in order. If found in a lower-priority storage, cache it in higher-priority storages.

### Request Processing Pipeline

Validate → Authorize → Log → Process → Transform → Return

### Data Transformation Chain

Parse → Validate → Enrich → Filter → Format → Serialize

## API Reference

### `IPipelineStep<TContext, TResult>`

Represents a single step in the pipeline. Implement this interface to create custom processing steps.

```csharp
Task<TResult> RunAsync(
    TContext context,
    PipeDelegate<TContext, TResult> next,
    CancellationToken ct = default);
```

### `IPipeline<TContext, TResult>`

The main pipeline interface used for dependency injection.

```csharp
Task<TResult> ExecuteAsync(TContext context, CancellationToken ct = default);
```

### `PipelineBuilder<TContext, TResult>`

Fluent builder for configuring pipelines.

```csharp
builder.AddStep<StepType>().Build();
```

### `IPipelineDiagnostics`

Optional interface for monitoring pipeline execution:

```csharp
void OnStepStarting(Type stepType, int stepIndex);
void OnStepCompleted(Type stepType, int stepIndex, TimeSpan duration);
void OnStepFailed(Type stepType, int stepIndex, Exception exception, TimeSpan duration);
void OnPipelineCompleted(TimeSpan duration, bool success);
```

## Best Practices

- **Keep steps focused** — Each step should have a single responsibility
- **Respect cancellation** — Honor the cancellation token for long-running operations
- **Handle exceptions gracefully** — Catch and handle errors appropriately in steps
- **Use dependency injection** — Steps can declare dependencies via constructor injection
- **Call next** — Remember to await `next()` unless intentionally short-circuiting
- **Immutable contexts** — Consider making contexts immutable to avoid unexpected mutations

## Examples

See the `samples/EasyPipe.WebApi` directory for complete working examples demonstrating:

- Basic pipeline execution with multiple steps
- Multi-step data transformation
- Cache lookup across multiple storage systems
- ASP.NET Core integration

## License

MIT

## Contributing

Contributions are welcome! Please feel free to submit pull requests or open issues on GitHub.

---