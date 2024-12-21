namespace Puzzles.Runner._2024;

using Map = Map2<char>;

[Puzzle("Keypad Conundrum", 21, 2024)]
public class Day21(ILinesInputReader input) : IPuzzleSolver
{
    private const char GAP = ' ';
    private const char ENTER = 'A';

    #region Members

    private Map _keyboard;
    private Map _robotKeyboard;

    private readonly Dictionary<(string, int), ulong> _cache = [];
    private readonly List<int> _pathBuffer = new(64);
    private readonly int[] _distanceBuffer = new int[64];

    #endregion

    public void Init()
    {
        _keyboard = Map.WithBorders(['7', '8', '9', '4', '5', '6', '1', '2', '3', GAP, '0', ENTER], 3, GAP);
        _robotKeyboard = Map.WithBorders([GAP, '^', ENTER, '<', 'v', '>'], 3, GAP);
    }

    public string SolvePart1()
        => Solve(2);

    public string SolvePart2()
        => Solve(25);

    #region Private methods

    private string Solve(int depth)
    {
        _cache.Clear();
        return input.Lines.UInt64Sum(code => SolveNumeric(code, depth)).ToString();
    }

    private ulong SolveNumeric(string code, int depth)
        => Convert.ToUInt64(code[..^1]) * AssembleSequence(_keyboard, code).UInt64Sum(ins => SolveDirectional(ins, depth));

    private ulong SolveDirectional(string code, int depth)
    {
        var tuple = (code, depth);

        if (_cache.TryGetValue(tuple, out var value))
            return value;

        if (depth == 0)
            return (ulong)code.Length;

        return _cache.AddAndReturn(tuple, AssembleSequence(_robotKeyboard, code)
            .UInt64Sum(ins => SolveDirectional(ins, depth - 1)));
    }

    private IEnumerable<string> AssembleSequence(Map keyboard, string code)
    {
        var location = Array.IndexOf(keyboard.Data, ENTER);

        foreach (var symbol in code)
        {
            var target = Array.IndexOf(keyboard.Data, symbol);

            if (target == location)
            {
                yield return ENTER.ToString();
                continue;
            }

            yield return GetInstructions(keyboard, location, target);
            location = target;
        }
    }

    private string GetInstructions(Map keyboard, int start, int end)
    {        
        var distances = BFS(keyboard, start, end, _distanceBuffer);

        _pathBuffer.Clear();
        ReconstructPath(keyboard, distances, end, start, _pathBuffer);

        var instructions = _pathBuffer.Reverse<int>()
            .Select(keyboard.InvDdx)
            .OrderBy(Ddx2W)
            .ToArray();

        if (!IsPossible(keyboard, start, instructions))
            instructions = [.. instructions.OrderByDescending(Ddx2W)];

        return new string(instructions.Select(Ddx2C).ToArray()) + ENTER.ToString();
    }

    private static bool IsPossible(Map keyboard, int location, int[] path)
    {
        var possibility = true;

        for (int i = 0; possibility && i < path.Length; i++)
        {
            location = keyboard.Next(location, path[i]);
            possibility &= keyboard[location] != GAP;
        }

        return possibility;
    }

    private static void ReconstructPath(Map map, int[] distances, int start, int end, List<int> path)
    {
        if (start == end)
            return;

        var next = Enumerable.Range(0, map.Directions.Length)
            .Select(ddx => (ddx, loc: map.Next(start, ddx)))
            .OrderBy(n => distances[n.loc])
            .First();

        path.Add(next.ddx);
        ReconstructPath(map, distances, next.loc, end, path);
    }

    private static int[] BFS(Map map, int start, int end, int[] distances)
    {
        Array.Fill(distances, Int32.MaxValue);

        var queue = new PriorityQueue<int, int>();
        var visited = new HashSet<int>();

        queue.Enqueue(start, 0);
        distances[start] = 0;

        while (queue.TryDequeue(out var current, out var distance))
        {
            if (current == end)
                return distances;

            if (map[current] == GAP || visited.Contains(current))
                continue;

            visited.Add(current);
            Enumerable.Range(0, map.Directions.Length).ForEach(ddx =>
            {
                var next = map.Next(current, ddx);
                var nextD = distances[current] + 1;

                if (nextD < distances[next] && map[next] != GAP)
                {
                    distances[next] = nextD;
                    queue.Enqueue(next, nextD);
                }
            });
        }

        return distances;
    }

    #region Converters

    private static int Ddx2W(int ddx) => ddx switch
    {
        0 => 2,
        1 => 4,
        2 => 3,
        3 => 1,

        _ => throw new NotImplementedException()
    };

    private static char Ddx2C(int dir) => dir switch
    {
        0 => '^',
        1 => '>',
        2 => 'v',
        3 => '<',

        _ => throw new NotImplementedException()
    };

    #endregion
    #endregion
}
