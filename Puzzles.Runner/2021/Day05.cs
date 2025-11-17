using System.Text.RegularExpressions;

namespace Puzzles.Runner._2021;

[Puzzle("Hydrothermal Venture", 5, 2021)]
public partial class Day05(ILinesInputReader input) : IPuzzleSolver
{    
    private record Line(Vec2 Source, Vec2 Destination);
    private Line[] _lines = [];

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
    }

    public string SolvePart1()
        => CalculateOverlaps(_lines.Where(line => 
            line.Source.X == line.Destination.X || 
            line.Source.Y == line.Destination.Y)).ToString();

    public string SolvePart2()
        => CalculateOverlaps(_lines).ToString();       

    private int CalculateOverlaps(IEnumerable<Line> lines)
    {
        Dictionary<(int x, int y), int> _counts = [];

        foreach (var line in lines)
        {
            var (sx, sy) = line.Source;
            var (dx, dy) = line.Destination;

            int count = Math.Max(Math.Abs(dx - sx), Math.Abs(dy - sy));

            var dirx = Math.Sign(dx - sx);
            var diry = Math.Sign(dy - sy);

            for (int i = 0; i <= count; i++)
            {
                var x = sx + dirx * i;
                var y = sy + diry * i;

                _counts.TryAdd((x, y), 0);
                _counts[(x, y)]++;
            }
        }

        return _counts.Values.Count(v => v > 1);
    }

    [GeneratedRegex(@"(?<sx>\d+),(?<sy>\d+) -> (?<dx>\d+),(?<dy>\d+)")]
    private static partial Regex LineRegex();
}
