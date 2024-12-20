namespace Puzzles.Runner._2024;

using System.ComponentModel.DataAnnotations;
using System.Data;
using Map = Map2<char>;

[Puzzle("Linen Layout", 20, 2024)]
public class Day20(ILinesInputReader input) : IPuzzleSolver
{
    #region Constants

    private const char START = 'S';
    private const char END= 'E';

    private const char BORDER = '@';
    private const char OBSTRUCTION = '#';
    private const char EMPTY = '.';

    private const int NO_PATH = -1;

    #endregion

    private Map _map;

    public void Init() => _map = Map.WithBorders
    (
        data: [.. input.Lines.SelectMany(line => line)],
        columns: input.Lines.First().Length,
        borderValue: BORDER
    );
    

    public string SolvePart1()
    {
        Dictionary<int, int> counter = [];

        var depth = 2;

        var start = Array.IndexOf(_map.Data, START);
        var end = Array.IndexOf(_map.Data, END);

        var buffer = _map.CreateBuffer<int>();
        BFS(_map, start, buffer);

        for (int i = 0; i < _map.Data.Length; i++)
        {
            if (_map[i] == BORDER || _map[i] == OBSTRUCTION)
                continue;

            var cells = GetCheatCells(_map, i, depth, []).ToArray();
            var od = cells.Select(c => (buffer[c] - buffer[i] - D(_map, i, c))).Where(d => d > 0).ForEach(d =>
            {
                counter.TryAdd(d, 0);
                counter[d]++;
            }).ToArray();
        }

        return counter.Where(kv => kv.Key >= 100).Sum(kv => kv.Value).ToString();

    }

    public string SolvePart2()
    {
        Dictionary<int, int> counter = [];

        var depth = 20;

        var start = Array.IndexOf(_map.Data, START);
        var end = Array.IndexOf(_map.Data, END);

        var buffer = _map.CreateBuffer<int>();
        BFS(_map, start, buffer);

        for (int i = 0; i < _map.Data.Length; i++)
        {
            if (_map[i] == BORDER || _map[i] == OBSTRUCTION)
                continue;

            var cells = GetCheatCells(_map, i, depth, []).ToArray();
            var od = cells.Select(c => (buffer[c] - buffer[i] - D(_map, i, c))).Where(d => d > 0).ForEach(d =>
            {
                counter.TryAdd(d, 0);
                counter[d]++;
            }).ToArray();
        }

        return counter.Where(kv => kv.Key >= 100).Sum(kv => kv.Value).ToString();

    }

    #region Private methods

    private static IEnumerable<int> GetCheatCells(Map map, int loc, int depth, HashSet<int> visited) 
    {
        var queue = new Queue<(int loc, int d)>();

        queue.Enqueue((loc, 0));        

        while (queue.TryDequeue(out var data))
        {
            var (current, distance) = data;

            if (map[current] == BORDER || visited.Contains(current))
                continue;

            visited.Add(current);

            if ((map[current] == EMPTY || map[current] == END) && distance > 0)
                yield return current;

            if (distance >= depth)
                continue;

            Array.ForEach(map.Directions, d => queue.Enqueue((current + d, distance + 1)));
        }
    }


    private static int BFS(Map map, int start, int[] distances)
    {
        Array.Fill(distances, Int32.MaxValue / 2);

        var queue = new Queue<int>();
        var visited = new HashSet<int>();

        queue.Enqueue(start);
        distances[start] = 0;

        while (queue.TryDequeue(out var current))
        {
            if (map[current] == OBSTRUCTION || map[current] == BORDER || visited.Contains(current))
                continue;

            visited.Add(current);
            Array.ForEach(map.Directions, d =>
            {
                var next = current + d;
                var nextD = distances[current] + 1;

                if (nextD < distances[next])
                {
                    distances[next] = nextD;
                    queue.Enqueue(next);
                }
            });
        }

        return NO_PATH;
    }

    private static int D(Map map, int from, int to)
    {
        var (cx, cy) = map.D1toD2(from);
        var (ex, ey) = map.D1toD2(to);

        return Math.Abs(cx - ex) + Math.Abs(cy - ey);
    }

    #endregion
}
