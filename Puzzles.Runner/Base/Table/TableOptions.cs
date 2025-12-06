namespace Puzzles.Runner.Base.Table;

[Flags]
public enum TableOptions
{
    None = 0,
    Header = 1,
    Separator = 2,
    Borders = 4,

    All = Header | Separator | Borders
}
