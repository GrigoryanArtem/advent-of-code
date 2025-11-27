using System.Text.RegularExpressions;

namespace Puzzles.Runner._2020;

using BagsTree = Dictionary<string, Dictionary<string, int>>;

[Puzzle("Handy Haversacks", 7, 2020)]
public partial class Day07(ILinesInputReader input) : IPuzzleSolver
{
    private const string SHINY_GOLD = "shiny gold";
    private BagsTree _tree = [];

    public void Init()
    {
        _tree = [];
        foreach (var (line, idx) in input.Lines.WithIndex())
        {
            var match = BagRegex().Match(line);

            _tree.Add
            (
                match.Groups["name"].Value,
                match.Groups["bag_num"].Captures
                    .Zip(match.Groups["bag_name"].Captures, 
                        (c1, c2) => (num: c1.Value, name: c2.Value))
                    .ToDictionary(d => d.name, d => Convert.ToInt32(d.num))
            );
        }
    }

    public string SolvePart1()
    {
        var mem = new Dictionary<string, bool>();
        return _tree.Keys.Count(n => CanHold(_tree, n, SHINY_GOLD, mem)).ToString();
    }

    public string SolvePart2()
        => ContainCount(_tree, SHINY_GOLD, []).ToString();
    
    private static bool CanHold(BagsTree tree, string source, string target, Dictionary<string, bool> mem)
    {
        if (mem.TryGetValue(source, out var value))
            return value;

        return mem.AddAndReturn(source, tree[source].ContainsKey(target) ||
            tree[source].Keys.Any(k => CanHold(tree, k, target, mem)));
    }

    private static int ContainCount(BagsTree tree, string target, Dictionary<string, int> mem)
    {
        if (mem.TryGetValue(target, out var count))
            return count;

        if (tree[target].Count == 0)
            return mem.AddAndReturn(target, 0);

        return mem.AddAndReturn(target, tree[target].Sum(v => v.Value + v.Value * ContainCount(tree, v.Key, mem)));
    }

    [GeneratedRegex(@"(?<name>.*?)\s+bags contain(\,?\s+(?<bag_num>\d+)\s+(?<bag_name>.*?)\s+bags?)*", RegexOptions.Compiled)]
    private static partial Regex BagRegex();
}
