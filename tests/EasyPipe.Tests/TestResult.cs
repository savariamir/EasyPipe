namespace EasyPipe.Tests;

/// <summary>
/// Test result for typed pipelines
/// </summary>
public class TestResult
{
    public string ResultValue { get; set; } = "";
    public int ResultCounter { get; set; }
    public bool Success { get; set; }
}