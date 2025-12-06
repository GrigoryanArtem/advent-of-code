namespace Puzzles.Runner.Base.Table;

public class Column
{
    public string Header { get; init; } = String.Empty;
    public int? Width { get; init; }
    public Align Align { get; init; } = Align.Left;
    public string? Format { get; init; }

}
