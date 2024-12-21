namespace Puzzles.Runner._2024;

using System.Dynamic;
using System.Text;
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


    public void DisassembleSequence(string seq)
    {        
        var c3 = ParseString(_robotKeyboard, seq);
        var c2 = ParseString(_robotKeyboard, c3);
        var c1 = ParseString(_keyboard, c2);

        Console.WriteLine();

        Console.WriteLine($">>> {seq} <<<");
        Console.WriteLine(c3);
        Console.WriteLine(c2);
        Console.WriteLine(c1);

        Console.WriteLine();
    }

    private string ParseString(Map keyboard, string seq)
    {
        List<char> answer = [];
        var location = Array.IndexOf(keyboard.Data, ENTER);
        foreach (var ch in seq)
        {
            if (ch == ENTER)
            {
                answer.Add(keyboard[location]);
                continue;
            }

            var ddx = C2Ddx(ch);
            location = keyboard.Next(location, ddx);
        }

        return new string([.. answer]);
    }

    public string SolvePart2()
    { 
        return "";
    }

    public string SolvePart1()
    {
        var sum = 0;
        foreach (var code in input.Lines)
        {
            var num = Convert.ToInt32(code[..^1]);

            var c1 = AssembleSequence(_keyboard, code.ToCharArray());
            var c2 = AssembleSequence(_robotKeyboard, c1.ToCharArray());
            var c3 = AssembleSequence(_robotKeyboard, c2.ToCharArray());

            sum += c3.Length * num;
        }

        return sum.ToString();
    }

    
    private static string AssembleSequence(Map keyboard, char[] code)
    {
        var buffer = keyboard.CreateBuffer<int>();
        var location = Array.IndexOf(keyboard.Data, ENTER);

        StringBuilder answer = new();

        foreach (var c in code)
        {
            var target = Array.IndexOf(keyboard.Data, c);

            if (target == location)
            {
                answer.Append(ENTER);
                continue;
            }

            var distances = Full(keyboard, location, target, buffer);

            var path = new List<int>();
            GetPath(keyboard, distances, target, location, path);

            answer.Append(GetPath(keyboard, location, target));

            location = target;
        }

        return answer.ToString();
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

    public static int[] Full(Map map,int start, int end, int[] distances)
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
