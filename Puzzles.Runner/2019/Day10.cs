namespace Puzzles.Runner._2019;
[Puzzle("Monitoring Station", 10, 2019)]
public class Day10(ILinesInputReader input) : IPuzzleSolver
{
    private const char ASTEROID = '#';
    
    private Vec2[] _asteroids = [];

    public void Init()
        => _asteroids = input.Lines
            .SelectMany((line, y) => line.Select((c, x) => (c, x, y)))
            .Where(t => t.c == ASTEROID)
            .Select(t => new Vec2(t.x, t.y))
            .ToArray();

    public string SolvePart1()
    {
        var data = Visibility(_asteroids);
        return data.Max(kv => kv.Value.GroupBy(x => x.Value.angle).Count()).ToString();
    }

    public string SolvePart2()
    {
        var data = Visibility(_asteroids);
        var laserPosition = data.MaxBy(kv => kv.Value.GroupBy(x => x.Value.angle).Count()).Key;        
        var dat = Order(data[laserPosition]);

        var (x, y) = dat[199];
        return (x * 100 + y).ToString();
    }

    private static Vec2[] Order(Dictionary<Vec2, (double angle, double distance)> data)
        => data
            .GroupBy(x => x.Value.angle)
            .SelectMany(x => x.Select((kv, idx) => (loc: kv.Key, angle: x.Key + (idx * AOC.PI2))))
            .OrderBy(x => x.angle)
            .Select(x => x.loc)
            .ToArray();


    private static Dictionary<Vec2, Dictionary<Vec2, (double angle, double distance)>> Visibility(Vec2[] asteroids)
    {
        Dictionary<Vec2, Dictionary<Vec2, (double angle, double distance)>> data = [];

        foreach (var ast in asteroids)
        {
            data.TryAdd(ast, []);

            foreach (var trg in asteroids)
            {
                if (trg == ast || data[ast].ContainsKey(trg))
                    continue;

                data.TryAdd(trg, []);
                
                var distance = AOC.EuclideanDistance(ast, trg);

                data[ast].Add(trg, (Angle(ast, trg), distance));
                data[trg].Add(ast, (Angle(trg, ast), distance));
            }
        }

        return data;
    }

    private static double Angle(Vec2 from, Vec2 to)
    {
        var angle = AOC.Angle(from, to) + AOC.HALF_PI;
        return angle < 0 ? angle + AOC.PI2 : angle;
    }
}
