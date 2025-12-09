namespace Puzzles.Runner._2025;

[Puzzle("Movie Theater", 9, 2025)]
public class Day09(ILinesInputReader input) : IPuzzleSolver
{
    private record struct Line(Vec2 A, Vec2 B);

    private Vec2[] _points = [];
    private Line[] _lines = [];

    public void Init()
    { 
        _points = [.. input.GetTokens(",", Int32.Parse)
            .Select(tokens => new Vec2(tokens[0], tokens[1]))];
        var n = _points.Length;

        _lines = new Line[n];
        for (int i = 0; i < n; i++)
            _lines[i] = new Line(_points[i], _points[(i + 1) % n]);
    }

    public string SolvePart1()
        => FindMaxArea(_points).ToString();

    public string SolvePart2()
        => FindInternalMaxArea(_points, _lines).ToString();

    private static ulong FindInternalMaxArea(Vec2[] points, Line[] lines)
    {        
        var max = 0UL;
        for (int i = 0; i < points.Length; i++)
        {
            for (int k = i + 1; k < points.Length; k++)
            {
                var probe = Area((points[i] - points[k]).Abs());

                if (probe > max)
                {
                    var hasIntersection = false;
                    for (int lidx = 0; lidx < lines.Length && !hasIntersection; lidx++)
                        hasIntersection |= Intersects(lines[lidx].A, lines[lidx].B, points[i], points[k]);
                    
                    max = hasIntersection ? max : probe;
                }
            }
        }

        return max;
    }

    private static ulong FindMaxArea(Vec2[] points)
    {
        var max = 0UL;
        for (int i = 0; i < points.Length; i++)
            for (int k = i + 1; k < points.Length; k++)
                max = Math.Max(max, Area((points[i] - points[k]).Abs()));

        return max;
    }

    private static ulong Area(Vec2 size)
        => (ulong)(size.X + 1) * (ulong)(size.Y + 1);

    private static bool Intersects(Vec2 l1a, Vec2 l1b, Vec2 l2a, Vec2 l2b)
    {        
        var (l1mix, l1max) = MinMax(l1a.X, l1b.X);        
        var (l1miy, l1may) = MinMax(l1a.Y, l1b.Y);

        var (l2mix, l2max) = MinMax(l2a.X, l2b.X);
        var (l2miy, l2may) = MinMax(l2a.Y, l2b.Y);

        return l2max > l1mix && l2mix < l1max && l2may > l1miy && l2miy < l1may;
    }

    private static (int min, int max) MinMax(int a, int b)
        => a < b ? (a, b) : (b, a);
}
