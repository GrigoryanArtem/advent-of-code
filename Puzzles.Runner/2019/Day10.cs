namespace Puzzles.Runner._2019;

using Map = Map2<char>;

[Puzzle("Monitoring Station", 10, 2019)]
public class Day10(ILinesInputReader input) : IPuzzleSolver
{
    private const char ASTEROID = '#';

    private Map _map = Map.Null;
    private int[] _asteroids = [];

    public void Init()
    {
        _map = new Map
        (
            data: [.. input.Lines.SelectMany(line => line)],
            columns: input.Lines.First().Length
        );

        _asteroids = [.._map.WithIndex().Where(p => p.item == ASTEROID).Select(p => p.index)];
    }

    public string SolvePart1()
    {
        var data = Visibility(_asteroids);
        return data.Max(kv => kv.Value.GroupBy(x => x.Value.angle).Count()).ToString();
    }

    public string SolvePart2()
    {
        var data = Visibility(_asteroids);
        var laserPosition = data.MaxBy(kv => kv.Value.GroupBy(x => x.Value.angle).Count()).Key;
        var lv = data[laserPosition].OrderBy(x => NormAngle(x.Value.angle)).ThenBy(kv => kv.Value.distance).ToArray();
        var dat = lv.Select(kv => _map.D1toD2(kv.Key)).ToArray();
        // var (x, y) = _map.D1toD2(lv[200].Key);

        // return (x * 100 + y).ToString();
        return "";
    }

    public Dictionary<int, Dictionary<int, (double angle, double distance)>> Visibility(int[] asteroids)
    {
        Dictionary<int, Dictionary<int, (double angle, double distance)>> data = [];

        foreach (var asteroid in asteroids)
        {
            data.TryAdd(asteroid, []);
            var (ax, ay) = _map.D1toD2(asteroid);

            foreach (var target in asteroids)
            {
                if (target == asteroid || data[asteroid].ContainsKey(target))
                    continue;

                data.TryAdd(target, []);

                var (tx, ty) = _map.D1toD2(target);

                var angle = AOC.Angle(ax, ay, tx, ty);
                var distance = AOC.EuclideanDistance(ax, ay, tx, ty);

                data[asteroid].Add(target, (angle, distance));
                data[target].Add(asteroid, (angle + Math.PI, distance));
            }
        }

        return data;
    }

    public static double NormAngle(double angle)
    {
        angle = angle - Math.PI / 2;
        return angle < 0 ? angle + 2 * Math.PI : angle;
    }
}
