namespace Puzzles.Runner._2024;

using Map = Map2<char>;

[Puzzle("Keypad Conundrum", 21, 2024)]
public class Day21(ILinesInputReader input) : IPuzzleSolver
{
    private const char GAP = ' ';
    private const char ENTER = 'A';

    private Map _keyboard;
    private Map _robotKeyboard;

    public void Init()
    {
        _keyboard = Map.WithBorders(['7', '8', '9', '4', '5', '6', '1', '2', '3', GAP, '0', ENTER], 3, GAP);
        _robotKeyboard = Map.WithBorders([GAP, '^', ENTER, '<', 'v', '>'], 3, GAP);
    }


    public string SolvePart1()
    {
        var sum = 0UL;
        foreach (var code in input.Lines)
        {
            sum += SolveNumeric(code, 2);
        }

        return sum.ToString();
    }
    public string SolvePart2()
    {
        var sum = 0UL;
        foreach (var code in input.Lines)
        {
            sum += SolveNumeric(code, 25);
        }

        return sum.ToString();
    }


    public ulong SolveNumeric(string code, int depth)
    {
        var num = Convert.ToUInt64(code[..^1]);

        var sum = 0UL;
        foreach (var instruction in AssembleSequence(_keyboard, code))
        {
            sum += num * SolveDirectional(instruction, depth);
        }

        return sum;
    }

    private readonly Dictionary<(string, int), ulong> _cache = [];
    public ulong SolveDirectional(string code, int depth)
    {
        var tuple = (code, depth);
        if(_cache.TryGetValue(tuple, out var value))
            return value;

        if (depth == 0)
            return (ulong)code.Length;

        var sum = 0UL;
        foreach (var instruction in AssembleSequence(_robotKeyboard, code))
        {
            sum += SolveDirectional(instruction, depth - 1);
        }

        return _cache.AddAndReturn(tuple, sum);
    }

    private static IEnumerable<string> AssembleSequence(Map keyboard, string code)
    {
        var buffer = keyboard.CreateBuffer<int>();
        var location = Array.IndexOf(keyboard.Data, ENTER);

        foreach (var c in code)
        {
            var target = Array.IndexOf(keyboard.Data, c);

            if (target == location)
            {
                yield return ENTER.ToString();
                continue;
            }

            var distances = Full(keyboard, location, target, buffer);

            var path = new List<int>();
            GetPath(keyboard, distances, target, location, path);

            yield return GetPath(keyboard, location, target);

            location = target;
        }
    }

    public static string GetPath(Map keyboard, int start, int end)
    {
        var buffer = keyboard.CreateBuffer<int>();
        var distances = Full(keyboard, start, end, buffer);

        var path = new List<int>();
        GetPath(keyboard, distances, end, start, path);

        var temp = path.Reverse<int>()
            .Select(keyboard.InvDdx)
            .OrderBy(Ddx2W2)
            .ToArray();

        if (!IsPossible(keyboard, start, end, temp))
            temp = [.. temp.OrderByDescending(Ddx2W2)];

        return new string(temp.Select(Ddx2C).ToArray()) + ENTER.ToString();
    }

    public static bool IsPossible(Map keyboard, int start, int end, int[] path)
    {
        var loc = start;

        foreach (var ddx in path)
        {
            loc = keyboard.Next(loc, ddx);

            if (keyboard[loc] == GAP)
                return false;
        }


        return true;
    }

    public static void GetPath(Map map, int[] distances, int start, int end, List<int> path)
    {
        if (start == end)
            return;

        var next = Enumerable.Range(0, map.Directions.Length)
            .Select(ddx => (ddx, loc: map.Next(start, ddx)))
            .OrderBy(n => distances[n.loc])
            .First();

        path.Add(next.ddx);
        GetPath(map, distances, next.loc, end, path);
    }

    public static char Ddx2C(int dir) => dir switch
    {
        0 => '^',
        1 => '>',
        2 => 'v',
        3 => '<',

        _ => throw new NotImplementedException()
    };

    public static int C2Ddx(char c) => c switch
    {
        '^' => 0,
        '>' => 1,
        'v' => 2,
        '<' => 3,

        _ => throw new NotImplementedException()
    };

    public static int[] Full(Map map, int start, int end, int[] distances)
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

    public static int Ddx2W2(int ddx) => ddx switch
    {
        0 => 2,
        1 => 4,
        2 => 3,
        3 => 1,

        _ => throw new NotImplementedException()
    };
}
