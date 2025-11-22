namespace EasyPipe.Tests;

public readonly struct Unit
{
    /// <summary>Singleton instance of Unit</summary>
    public static readonly Unit Value = default;

    public override string ToString() => "()";
}