using System.Collections.Concurrent;

namespace Puzzles.Runner._2024;

[Puzzle("Linen Layout", 19, 2024)]
public class Day19(ILinesInputReader input) : IPuzzleSolver
{
    private ConcurrentDictionary<string, ulong> _cache = [];

    private string[] _towels = [];
    private string[] _patterns = [];

    public void Init()
    {
        var tokens = input.GetTokens(",", s => s);

        _towels = tokens[0];
        _patterns = tokens.Skip(2).Select(t => t.First()).ToArray();
    }

    public string SolvePart1()
    {
        _cache = new(_towels.ToDictionary(t => t, _ => 1UL));
        return _patterns.AsParallel().Count(p => DesignCombinations(p) > 0UL).ToString();
    }

    public string SolvePart2()
        => _patterns.UInt64Sum(DesignCombinations).ToString();

    public ulong DesignCombinations(string pattern)
        => _cache.GetOrAdd(pattern, _towels.Where(pattern.StartsWith)
            .UInt64Sum(t => DesignCombinations(pattern[t.Length..])));
}
