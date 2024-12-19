namespace Puzzles.Runner._2024;

[Puzzle("Linen Layout", 19, 2024)]
public class Day19(ILinesInputReader input) : IPuzzleSolver
{
    private readonly Dictionary<string, ulong> _cache = [];

    private HashSet<string> _towels = [];
    private string[] _patterns = [];

    public void Init()
    {
        var tokens = input.GetTokens(",", s => s);

        _towels = [.. tokens[0]];
        _patterns = tokens.Skip(2).Select(t => t.First()).ToArray();
    }

    public string SolvePart1()
    {
        _cache.Clear();
        return _patterns.Count(p => DesignCombinations(p) > 0UL).ToString();
    }

    public string SolvePart2()
    {
        _cache.Clear();
        return _patterns.UInt64Sum(DesignCombinations).ToString();
    }

    public ulong DesignCombinations(string pattern)
        => _cache.TryGetValue(pattern, out var value) ? value :
            _cache.AddAndReturn(pattern, (_towels.Contains(pattern) ? 1UL : 0UL) +
                _towels.Where(t => t.Length < pattern.Length && pattern[..t.Length] == t)
                    .UInt64Sum(t => DesignCombinations(pattern[t.Length..])));
}
