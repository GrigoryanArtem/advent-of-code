namespace Puzzles.Runner._2022;

using Puzzles.Base;
using Map = Mat2<char>;

[Puzzle("Hill Climbing Algorithm", 12, 2022)]
public partial class Day12(ILinesInputReader input) : IPuzzleSolver
{
    private const char BORDER = '#';

    private Map _map = Map.Null;
    private int[] _buffer = [];

    private int _start;
    private int _end;

    public void Init()
    {
        _map = Map.WithBorders
        (
            [.. input.Lines.SelectMany(c => c.AsEnumerable())], 
            input.Lines.First().Length, 
            '#'
        );

        _start = Array.IndexOf(_map.Data, 'S');
        _end = Array.IndexOf(_map.Data, 'E');

        _map[_start] = 'a';
        _map[_end] = 'z';

        _buffer = _map.CreateBuffer<int>();
    }

    public string SolvePart1()
    {        
        DMat(_map, _end, _buffer);

        return _buffer[_start].ToString();
    }

    public string SolvePart2()
    {
        DMat(_map, _end, _buffer);

        return _map.WithIndex()
            .Where(d => d.item == 'a' && _buffer[d.index] > 0)
            .Min(d => _buffer[d.index])
            .ToString();
    }

    private static void DMat(Map map, int start, int[] distances)
    {
        Array.Fill(distances, Int32.MaxValue);

        var queue = new PriorityQueue<int, int>();
        var visited = new HashSet<int>();

        queue.Enqueue(start, 0);
        distances[start] = 0;

        while (queue.TryDequeue(out var current, out var _))
        {
            if (map[current] == BORDER || visited.Contains(current))
                continue;

            visited.Add(current);

            Array.ForEach(map.Directions, d =>
            {
                var next = current + d;
                var dh = map[current] - map[next];

                if (dh > 1)
                    return;

                var nextD = distances[current] + 1;

                if (distances[next] > nextD)
                {
                    distances[next] = nextD;
                    queue.Enqueue(next, nextD);
                }
            });
        }
    }
}
