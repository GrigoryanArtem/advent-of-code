namespace Puzzles.Runner._2025;

[Puzzle("Reactor", 11, 2025)]
internal class Day11(ILinesInputReader input) : IPuzzleSolver
{
    private Dictionary<int, int[]> _connections = [];
    private readonly Dictionary<string, int> _id = [];  

    public void Init()
    {        
        _id.Clear();

        _connections = input.GetTokens(": ", x => x).ToDictionary
        (
            x => Id(x[0]),
            x => x[1].Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(Id)
                .ToArray()
        );
    }

    public string SolvePart1()
        => PathsCount("you", "out", []).ToString();

    public string SolvePart2()
    {
        Dictionary<(int, int), int> mem = [];
        var count = 1L;

        count *= PathsCount("svr", "fft", mem);
        count *= PathsCount("fft", "dac", mem);
        count *= PathsCount("dac", "out", mem);

        return count.ToString();
    }

    public int PathsCount(string start, string end, Dictionary<(int, int), int> mem)
        => PathsCount(_id[start], _id[end], mem);

    public int PathsCount(int start, int end, Dictionary<(int, int), int> mem)
    {        
        var path = (start, end);

        if (mem.TryGetValue(path, out var count))
            return count;

        if (start == end)
            return mem.AddAndReturn(path, 1);
        
        return mem.AddAndReturn(path, _connections.GetValueOrDefault(start, [])
            .Sum(x => PathsCount(x, end, mem)));
    }

    private int Id(string node)
        => _id.GetOrAdd(node, _id.Count);
}
