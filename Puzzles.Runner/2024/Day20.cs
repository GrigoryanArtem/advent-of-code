namespace Puzzles.Runner._2024;

using Map = Mat2<char>;

[Puzzle("Race Condition", 20, 2024)]
public class Day20(ILinesInputReader input) : IPuzzleSolver
{
    #region Constants

    private const char START = 'S';
    private const char END = 'E';

    private const char BORDER = '@';
    private const char OBSTRUCTION = '#';
    private const char EMPTY = '.';

    #endregion

    private Map? _map;

    public void Init() => _map = Map.WithBorders
    (
        data: [.. input.Lines.SelectMany(line => line)],
        columns: input.Lines.First().Length,
        borderValue: BORDER
    );

    public string SolvePart1()
        => Solve(_map!, 2, 100).ToString();

    public string SolvePart2()
        => Solve(_map!, 20, 100).ToString();

    #region Private methods

    private static int Solve(Map map, int depth, int minDistance)
    {
        var bfs = new BFS<char>(map, [OBSTRUCTION, BORDER]);

        var start = Array.IndexOf(map.Data, START);
        var end = Array.IndexOf(map.Data, END);

        var te = bfs.Full(start, map.CreateBuffer<int>());
        var ts = bfs.Full(end, map.CreateBuffer<int>());

        return map.WithIndex()
            .Where(c => c.item != BORDER && c.item != OBSTRUCTION)
            .AsParallel()
            .Sum(cell => GetCheatCells(map, cell.index, depth, [])
                .Select(jmp => te[end] - (te[cell.index] + ts[jmp.loc] + jmp.dist))
                .Count(dst => dst >= minDistance));
    }

    private static IEnumerable<(int loc, int dist)> GetCheatCells(Map map, int loc, int depth, HashSet<int> visited)
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
                yield return (current, distance);

            if (distance >= depth)
                continue;

            Array.ForEach(map.Directions, d => queue.Enqueue((current + d, distance + 1)));
        }
    }

    #endregion
}
