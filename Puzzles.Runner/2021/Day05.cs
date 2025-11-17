using System.Text.RegularExpressions;

namespace Puzzles.Runner._2021;

using Mat = Mat2<int>;

[Puzzle("Hydrothermal Venture", 5, 2021)]
public partial class Day05(ILinesInputReader input) : IPuzzleSolver
{
    private record Line(Vec2 Start, Vec2 End);
    private Line[] _lines = [];

    private int _sizeX = 0;
    private int _sizeY = 0;

    public void Init()
    {
        _lines = new Line[input.Lines.Length];

        foreach (var (line, idx) in input.Lines.WithIndex())
        {
            var match = LineRegex().Match(line);

            var sx = Convert.ToInt32(match.Groups["sx"].Value);
            var sy = Convert.ToInt32(match.Groups["sy"].Value);

            var dx = Convert.ToInt32(match.Groups["dx"].Value);
            var dy = Convert.ToInt32(match.Groups["dy"].Value);

            _lines[idx] = new(new(sx, sy), new(dx, dy));
        }

        _sizeX = _lines.Max(l => Math.Max(l.Start.X, l.End.X)) + 1;
        _sizeY = _lines.Max(l => Math.Max(l.Start.Y, l.End.Y)) + 1;
    }

    public string SolvePart1()
        => CalculateOverlaps(_lines.Where(line =>
            line.Start.X == line.End.X ||
            line.Start.Y == line.End.Y)).ToString();

    public string SolvePart2()
        => CalculateOverlaps(_lines).ToString();

    private int CalculateOverlaps(IEnumerable<Line> lines)
    {
        var mat = Mat.Empty(_sizeX, _sizeY);

        lines.AsParallel().ForAll(line =>
        {
            var (sx, sy) = line.Start;
            var (ex, ey) = line.End;

            var dx = ex - sx;
            var dy = ey - sy;

            int count = Math.Max(Math.Abs(dx), Math.Abs(dy));

            var dirx = Math.Sign(dx);
            var diry = Math.Sign(dy);

            for (int i = 0; i <= count; i++)
                Interlocked.Increment(ref mat.Ref(sx + dirx * i, sy + diry * i));
        });

        return mat.Count(v => v > 1);
    }

    [GeneratedRegex(@"(?<sx>\d+),(?<sy>\d+) -> (?<dx>\d+),(?<dy>\d+)")]
    private static partial Regex LineRegex();
}

