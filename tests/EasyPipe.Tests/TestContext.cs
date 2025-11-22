namespace EasyPipe.Tests;

/// <summary>
/// Test context for typed pipelines
/// </summary>
public class TestContext
{
    public string Value { get; set; } = "";
    public List<string> ExecutedSteps { get; set; } = [];
    public CancellationToken? UsedCancellationToken { get; set; }
}