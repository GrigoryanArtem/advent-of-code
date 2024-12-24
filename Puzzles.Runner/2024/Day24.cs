using System.Text.RegularExpressions;

namespace Puzzles.Runner._2024;

[Puzzle("Crossed Wires", 24, 2024)]
public partial class Day24(IFullInputReader input) : IPuzzleSolver
{
    public record Rule(string Left, string Op, string Right, string Answer);

    private readonly Dictionary<string, bool> _values = [];
    private Rule[] _rules = [];

    public void Init()
    {
        _values.Clear();

        ValueRegex().Matches(input.Text)
            .ForEach(m => _values.Add(m.Groups["name"].Value, 
                Convert.ToInt16(m.Groups["value"].Value) > 0));

        _rules = OperationsRegex().Matches(input.Text)
            .Select(m => new Rule
            (
                Left: m.Groups["left"].Value,
                Op: m.Groups["op"].Value,
                Right: m.Groups["right"].Value,
                Answer: m.Groups["value"].Value
            )).ToArray();
    }

    public string SolvePart1()
    {
        var memory = new Dictionary<string, bool>(_values);

        for(var changed = true; changed;)
        {
            changed = false;

            foreach(var rule in _rules)
            {
                if (!memory.ContainsKey(rule.Answer) &&
                    memory.TryGetValue(rule.Left, out bool left) &&
                    memory.TryGetValue(rule.Right, out bool right))
                {
                    memory.Add(rule.Answer, Calculate(left, right, rule.Op));
                    changed = true;
                }
            }
        }

        return BitArray2UInt64(memory.Where(kv => kv.Key.StartsWith('z'))
            .OrderBy(kv => kv.Key)
            .Select(kv => kv.Value))
            .ToString();
    }

    private static bool Calculate(bool a, bool b, string op) => op switch
    {
        "AND" => a & b,
        "OR" => a | b,
        "XOR" => a ^ b,

        _ => throw new NotImplementedException(),
    };

    private static ulong BitArray2UInt64(IEnumerable<bool> arr)
        => arr.WithIndex().Where(b => b.item).Aggregate(0UL, (acc, b) => acc |= 1UL << b.index);
    

    [GeneratedRegex(@"(?<name>.*?)\:\s+(?<value>\d)", RegexOptions.Compiled)]
    private static partial Regex ValueRegex();
    [GeneratedRegex(@"(?<left>\S*)\s+(?<op>\S*)\s+(?<right>\S*)\s+->\s+(?<value>\S*)", RegexOptions.Compiled)]
    private static partial Regex OperationsRegex();
}
