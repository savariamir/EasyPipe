using System;

namespace EasyPipe.V2;

/// <summary>
/// Wraps pipeline execution results to distinguish between successful execution,
/// no result, and error scenarios.
/// </summary>
public class PipelineResult<TResult>
{
    public bool IsSuccess { get; }
    public TResult? Value { get; }
    public Exception? Exception { get; }
    public string? ErrorMessage { get; }

    private PipelineResult(bool isSuccess, TResult? value, Exception? exception, string? errorMessage)
    {
        IsSuccess = isSuccess;
        Value = value;
        Exception = exception;
        ErrorMessage = errorMessage;
    }

    public static PipelineResult<TResult> Success(TResult value) => 
        new(true, value, null, null);

    public static PipelineResult<TResult> NoResult() => 
        new(true, default, null, null);

    public static PipelineResult<TResult> Failure(Exception exception) => 
        new(false, default, exception, exception.Message);

    public static PipelineResult<TResult> Failure(string errorMessage) => 
        new(false, default, null, errorMessage);
}