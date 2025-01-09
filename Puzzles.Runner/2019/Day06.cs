namespace Puzzles.Runner._2019;

using System.Collections.Concurrent;

[Puzzle("Universal Orbit Map", 6, 2019)]
public class Day06(ILinesInputReader input) : IPuzzleSolver
{
    private readonly Dictionary<string, string> _graph = [];
    private readonly ConcurrentDictionary<string, int> _cache = [];

    public void Init()
        => input.GetTokens(")", x => x).ForEach(tkn => _graph.Add(tkn[1], tkn[0]));    

    public string SolvePart1()
    {
        _cache.Clear();
        return _graph.Keys.AsParallel().Sum(CheckSum).ToString();
    }

    public string SolvePart2()
        => Distance("YOU", "SAN").ToString();

    private int CheckSum(string planet)
        => _cache.GetOrAdd(planet, _graph.TryGetValue(planet, out var next) ? 1 + CheckSum(next) : 0);

    private int Distance(string from, string to)
    {
        _cache.Clear();
        
        var fromCheckSum = CheckSum(from);
        var distance = 0;

        while (!_cache.ContainsKey(to))
        {
            to = _graph[to];
            distance++;
        }

        return fromCheckSum - _cache[to] + distance - 2;
    }
}
