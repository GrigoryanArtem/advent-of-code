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

        foreach (var id in _id.Values)
            if (!_connections.ContainsKey(id))
                _connections.Add(id, []);
    }

    public string SolvePart1()
        => PathsCount("you", "out").ToString();

    public string SolvePart2()
    {        
        var count = 1L;

        count *= PathsCount("svr", "fft");
        count *= PathsCount("fft", "dac");
        count *= PathsCount("dac", "out");

        return count.ToString();
    }

    public long PathsCount(string start, string end)
        => PathsCount(Id(start), Id(end));

    public long PathsCount(int start, int end)
    {
        Span<long> counts = stackalloc long[_connections.Count];
        counts[end] = 1;
        
        foreach (var curr in Sort(start))
        {            
            var sum = 0L; 
            foreach(var next in _connections[curr])
                sum += counts[next];

            counts[curr] += sum;
        }

        return counts[start];
    }

    public List<int> Sort(int start)
    {
        var list = new List<int>(_connections.Count);

        Span<bool> visited = stackalloc bool[_connections.Count];
        Span<int> stack = stackalloc int[1024];
        var sidx = 0;

        stack[sidx++] = start;

        while (sidx > 0)
        {
            var node = stack[--sidx];
            var curr = Math.Abs(node);

            if (!visited[curr])
            {
                visited[curr] = true;
                stack[sidx++] = -curr;

                foreach (var next in _connections[curr])
                    if (!visited[next])
                        stack[sidx++] = next;
            }

            if (node < 0)
                list.Add(curr);
        }

        return list;
    }

    private int Id(string node)
        => _id.GetOrAdd(node, _id.Count);
}
