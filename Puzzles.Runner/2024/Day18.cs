namespace Puzzles.Runner._2024;

using Map = Mat2<char>;

[Puzzle("RAM Run", 18, 2024)]
public class Day18(ILinesInputReader input) : IPuzzleSolver
{
    #region Constants

    private const char BORDER = '#';
    private const char EMPTY = '.';

    private const int NO_PATH = -1;

    #endregion

    private (int x, int y)[] _obstructions = [];

    public void Init()
        => _obstructions = input.GetTokens(",", Convert.ToInt32)
            .Select(t => (t[0], t[1]))
            .ToArray();    

    public string SolvePart1()
    {
        var (map, start, end) = CreateMap(new(71, 71));
        CorruptMap(map, 1024);

        return FindPath(map, start, end, map.CreateBuffer<int>()).ToString();
    }

    public string SolvePart2()
    {
        var (map, start, end) = CreateMap(new(71, 71));
        var buffer = map.CreateBuffer<int>();

        var (head, tail) = (1024 + 1, _obstructions.Length);
        while (tail - head > 1)
        {
            var mid = (head + tail) / 2;

            var mapCopy = map.Copy();
            CorruptMap(mapCopy, mid);

            if (FindPath(mapCopy, start, end, distances: buffer) == NO_PATH)
            {
                tail = mid;
            }
            else
            {
                head = mid;
            }
        }

        var (x, y) = _obstructions[head];
        return $"{x},{y}";
    }

    #region Private methods

    private static int FindPath(Map map, int start, int end, int[] distances)
    {
        Array.Fill(distances, Int32.MaxValue);        

        var queue = new PriorityQueue<int, int>();
        var visited = new HashSet<int>();

        queue.Enqueue(start, H(map, start, end));        
        distances[start] = 0;

        while (queue.TryDequeue(out var current, out var _))
        {
            if (current == end)
                return distances[current];

            if (map[current] == BORDER || visited.Contains(current))
                continue;

            visited.Add(current);

            Array.ForEach(map.Directions, d =>
            {
                var next = current + d;
                var nextD = distances[current] + 1;

                if (distances[next] > nextD)
                {
                    distances[next] = nextD;
                    queue.Enqueue(next, nextD + H(map, next, end));
                }
            });
        }

        return NO_PATH;
    }

    private static int H(Map map, int from, int to)
    {
        var (cx, cy) = map.D1toD2(from);
        var (ex, ey) = map.D1toD2(to);

        return Math.Abs(cx - ex) + Math.Abs(cy - ey);
    }

    private void CorruptMap(Map map, int steps)
        => _obstructions.Take(steps).ForEach(p => map[p.x +1, p.y + 1] = BORDER);

    private static (Map map, int start, int end) CreateMap(Vec2 size)
    {
        var map = new Map(data: new char[(size.X + 2) * (size.Y + 2)], size.X + 2);

        Array.Fill(map.Data, EMPTY);
        map.FillBorders(BORDER);

        var start = map.D2toD1(1, 1);
        var end = map.D2toD1(size.X, size.Y);

        return (map, start, end);
    }

    #endregion
}
