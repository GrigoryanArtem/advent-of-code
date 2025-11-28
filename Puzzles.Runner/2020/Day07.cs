using System.Text.RegularExpressions;

namespace Puzzles.Runner._2020;

using BagsTree = Dictionary<int, Dictionary<int, int>>;

[Puzzle("Handy Haversacks", 7, 2020)]
public partial class Day07(ILinesInputReader input) : IPuzzleSolver
{
    private const string SHINY_GOLD = "shiny gold";
    
    private BagsTree _tree = [];
    private readonly Dictionary<string, int> _ids = [];

    public void Init()
    {
        _tree = [];
        foreach (var (line, idx) in input.Lines.WithIndex())
        {
            var match = BagRegex().Match(line);
            
            _tree.Add
            (
                Id(match.Groups["name"].Value),
                match.Groups["bag_num"].Captures
                    .Zip(match.Groups["bag_name"].Captures, 
                        (c1, c2) => (num: c1.Value, name: c2.Value))
                    .ToDictionary(d => Id(d.name), d => Convert.ToInt32(d.num))
            );
        }
    }

    public string SolvePart1()
    {
        var rev = new List<int>[_ids.Count];
        for (int i = 0; i < rev.Length; i++)
            rev[i] = [];

        foreach (var (source, holds) in _tree)         
            foreach (var (bag, _) in holds)
                rev[bag].Add(source);

        return HoldCount(rev, Id(SHINY_GOLD)).ToString();
    }

    public string SolvePart2()
        => ContainCount(_tree, Id(SHINY_GOLD), []).ToString();

    public static int HoldCount(List<int>[] rev, int target)
    {
        Queue<int> q = [];
        HashSet<int> set = [];

        q.Enqueue(target);

        while (q.Count > 0)
        {
            var bag = q.Dequeue();
            foreach (var hold in rev[bag])
                if(set.Add(hold))
                    q.Enqueue(hold);
        }

        return set.Count;
    }

    private static int ContainCount(BagsTree tree, int target, Dictionary<int, int> mem)
    {
        if (mem.TryGetValue(target, out var count))
            return count;

        if (tree[target].Count == 0)
            return mem.AddAndReturn(target, 0);

        return mem.AddAndReturn(target, tree[target].Sum(v => v.Value + v.Value * ContainCount(tree, v.Key, mem)));
    }

    [GeneratedRegex(@"(?<name>.*?)\s+bags contain(\,?\s+(?<bag_num>\d+)\s+(?<bag_name>.*?)\s+bags?)*", RegexOptions.Compiled)]
    private static partial Regex BagRegex();

    private int Id(string name)
        => _ids.GetOrAdd(name, _ids.Count);
}
