namespace Puzzles.Runner._2017;

[Puzzle("Hex Ed", 11, 2017)]
public class Day11(IFullInputReader input) : IPuzzleSolver
{
    private static readonly Dictionary<string, HexAx> _dirs = new()
    {
        ["n"] = new(0, -1),
        ["s"] = new(0, +1),

        ["ne"] = new(+1, -1),
        ["sw"] = new(-1, +1),

        ["se"] = new(+1, 0),
        ["nw"] = new(-1, 0),
    };

    public string SolvePart1()
    {
        var pos = HexAx.Zero;
        foreach (var dir in input.Text.Split(','))
            pos.Add(_dirs[dir]);

        return pos.HexLength().ToString();
    }

    public string SolvePart2()
    {
        var pos = HexAx.Zero;
        var max = 0;
        foreach (var dir in input.Text.Split(','))
        {
            pos.Add(_dirs[dir]);
            max = Math.Max(pos.HexLength(), max);
        }

        return max.ToString();
    }
}
