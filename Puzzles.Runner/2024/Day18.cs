namespace Puzzles.Runner._2024;

using Puzzles.Base.Entites;
using Map = Puzzles.Base.Entites.Map2<char>;

[Puzzle("RAM Run", 18, 2024)]
public class Day18(ILinesInputReader input) : IPuzzleSolver
{
    private const char BORDER = '#';
    private const char EMPTY = '.';
    private const int NO_PATH = -1;

    private (int x, int y)[] _obstructions = [];

    public void Init()
        => _obstructions = input.GetTokens(",", Convert.ToInt32)
            .Select(t => (t[0] + 1, t[1] + 1))
            .ToArray();    

    public string SolvePart1()
    {
        var (map, start, end) = CreateMap(new(71, 71));
        CorruptMap(map, 1024);

        return FindPath(map, start, end).ToString();
    }

    public string SolvePart2()
    {
        var (map, start, end) = CreateMap(new(71, 71));

        var (head, tail) = (1024 + 1, _obstructions.Length);
        while (tail - head > 1)
        {
            var mid = (head + tail) / 2;

            var mapCopy = map.Copy();
            CorruptMap(mapCopy, mid);

            if (FindPath(mapCopy, start, end) == NO_PATH)
            {
                tail = mid;
            }
            else
            {
                head = mid;
            }
        }

        var (x, y) = _obstructions[head];
        return $"{x - 1},{y - 1}";
    }

    #region Private methods

    private static int FindPath(Map map, int start, int end)
    {
        var queue = new PriorityQueue<int, int>();
        var visited = new HashSet<int>();

        queue.Enqueue(start, 0);        
        while (queue.TryDequeue(out var current, out var distance))
        {            
            if (map[current] == BORDER || visited.Contains(current))
                continue;

            visited.Add(current);
            if (current == end)
                return distance;

            Array.ForEach(map.Directions, d => queue.Enqueue(current + d, distance + 1));
        }

        return NO_PATH;
    }

    private void CorruptMap(Map map, int steps)
        => _obstructions.Take(steps).ForEach(p => map[p.x, p.y] = BORDER);

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
