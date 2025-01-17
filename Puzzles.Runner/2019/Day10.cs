namespace Puzzles.Runner._2019;

[Puzzle("Monitoring Station", 10, 2019)]
public class Day10(ILinesInputReader input) : IPuzzleSolver
{
    private record VisibilityData(double Angle, double Distance);

    private const char ASTEROID = '#';
    private const int TARGET_ASTEROID = 199;

    private Vec2[] _asteroids = [];

    public void Init()
        => _asteroids = input.Lines
            .SelectMany((line, y) => line.Select((c, x) => (c, x, y)))
            .Where(t => t.c == ASTEROID)
            .Select(t => new Vec2(t.x, t.y))
            .ToArray();

    public string SolvePart1()
        => Visibility(_asteroids).Max(v => v.WhereNotNull().Select(d => d.Angle).Distinct().Count()).ToString();

    public string SolvePart2()
    {
        var visibility = Visibility(_asteroids);
        var laserPosition = visibility.IndexOfMax(v => v.WhereNotNull()
            .Select(d => d.Angle)
            .Distinct()
            .Count());

        var laserData = visibility[laserPosition]
            .Zip(_asteroids, (vd, a) => (vd, a))
            .WhereNotNull(d => d.vd)
            .Select(d => (d.a, d.vd.Angle, d.vd.Distance))
            .ToArray();

        var (x, y) = Order(laserData)[TARGET_ASTEROID];
        return (x * 100 + y).ToString();
    }

    private static Vec2[] Order(IEnumerable<(Vec2 position, double angle, double distance)> data)
        => data.GroupBy(x => x.angle)
            .SelectMany(x => x.Select((kv, idx) => (loc: kv.position, angle: x.Key + (idx * AOC.PI2))))
            .OrderBy(x => x.angle)
            .Select(x => x.loc)
            .ToArray();
    private static VisibilityData[][] Visibility(Vec2[] asteroids)
    {
        var visibility = new VisibilityData[asteroids.Length][];

        for (int i = 0; i < visibility.Length; i++)
            visibility[i] = new VisibilityData[asteroids.Length];

        asteroids.WithIndex().AsParallel().ForAll(a =>
        {
            for (int i = a.index, j = a.index + 1; j < asteroids.Length; j++)
            {
                var distance = AOC.EuclideanDistance(asteroids[i], asteroids[j]);

                visibility[i][j] = new(Angle(asteroids[i], asteroids[j]), distance);
                visibility[j][i] = new(Angle(asteroids[j], asteroids[i]), distance);
            }
        });

        return visibility;
    }

    private static double Angle(Vec2 from, Vec2 to)
    {
        var angle = AOC.Angle(from, to) + AOC.HALF_PI;
        return angle < 0 ? angle + AOC.PI2 : angle;
    }
}
