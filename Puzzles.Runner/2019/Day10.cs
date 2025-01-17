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

    public Vec2[] Order(IEnumerable<KeyValuePair<Vec2, (double angle, double distance)>> data)
    {
        var sort = data.OrderBy(x => NormAngle(x.Value.angle)).ThenBy(kv => kv.Value.distance).ToList();
        var result = new Vec2[sort.Count];

        for(int k = 0; k < result.Length;)
        {
            double? angle = null;
            for (int i = 0; i < sort.Count; i++)
            {
                if (angle == sort[i].Value.angle)
                    continue;

                angle = sort[i].Value.angle;
                result[k++] = sort[i].Key;
                sort.RemoveAt(i);
                i--;                                    
            }
        }

        return result;
    }

    public static Dictionary<Vec2, Dictionary<Vec2, (double angle, double distance)>> Visibility(Vec2[] asteroids)
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

                data[ast].Add(trg, (AOC.Angle(ast, trg), distance));
                data[trg].Add(ast, (AOC.Angle(trg, ast), distance));
            }
        }

        return data;
    }

    public static double NormAngle(double angle)
    {
        angle = angle + Math.PI / 2;
        angle = angle < 0 ? angle + 2 * Math.PI : angle;
        return angle * 180 / Math.PI;
    }
}
