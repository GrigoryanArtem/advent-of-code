using System.Text.RegularExpressions;

namespace Puzzles.Runner._2024;

[Puzzle("Crossed Wires", 24, 2024)]
public partial class Day24(IFullInputReader input) : IPuzzleSolver
{
    public enum Op { AND, OR, XOR };
    public record Rule(Op Op, string In1, string In2, string Out);

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
                Op: Enum.Parse<Op>(m.Groups["op"].Value),
                In1: m.Groups["in1"].Value,
                In2: m.Groups["in2"].Value,
                Out: m.Groups["out"].Value
            )).ToArray();
    }

    public string SolvePart1()
        => Run([]).ToString();

    public string SolvePart2()
        => String.Join(",", GenerateSwaps([]).Keys.Distinct().Order());

    #region Pirvate methods

    private Dictionary<string, string> GenerateSwaps(Dictionary<string, string> swaps)
    {
        void Swap(string a, string b)
        {
            swaps.Add(a, b);
            swaps.Add(b, a);
        }

        var count = Convert.ToInt32(_values.Keys
            .Where(k => k.StartsWith('x'))
            .Order()
            .Last()[1..]);

        var co = GetOut(swaps, "x00", "y00", Op.AND);
        for (int i = 1; i <= count; i++)
        {
            var x = $"x{i:D2}";
            var y = $"y{i:D2}";
            var z = $"z{i:D2}";

            var xor = GetOut(swaps, x, y, Op.XOR)!;
            var and = GetOut(swaps, x, y, Op.AND)!;

            var cXor = GetOut(swaps, co, xor, Op.XOR);
            var cAnd = GetOut(swaps, co, xor, Op.AND);

            if (cXor == null && cAnd == null)
            {
                Swap(xor, and);
                return GenerateSwaps(swaps);
            }

            if (cXor != z)
            {
                Swap(cXor!, z);
                return GenerateSwaps(swaps);
            }

            co = GetOut(swaps, and, cAnd, Op.OR);
        }
        return swaps;
    }

    private string? GetOut(Dictionary<string, string> swaps, string? in1, string? in2, Op op)
        => ApplySwaps(swaps, GetOut(in1, in2, op) ?? GetOut(in2, in1, op));
    private string? GetOut(string? in1, string? in2, Op op)
        => _rules.Where(r => r.In1 == in1 && r.In2 == in2 && r.Op == op).FirstOrDefault()?.Out;

    private ulong Run(Dictionary<string, string> swaps)
    {
        var memory = new Dictionary<string, bool>(_values);

        for (var changed = true; changed;)
        {
            changed = false;

            foreach (var rule in _rules)
            {
                var @out = ApplySwaps(swaps, rule.Out)!;

                if (!memory.ContainsKey(@out) &&
                    memory.TryGetValue(rule.In1, out bool in1) &&
                    memory.TryGetValue(rule.In2, out bool in2))
                {
                    memory.Add(@out, Calculate(in1, in2, rule.Op));
                    changed = true;
                }
            }
        }

        return BitArray2UInt64(memory.Where(kv => kv.Key.StartsWith('z'))
            .OrderBy(kv => kv.Key)
            .Select(kv => kv.Value));
    }

    private static string? ApplySwaps(Dictionary<string, string> swaps, string? @out)
        => @out is not null && swaps.TryGetValue(@out, out var result) ? result : @out;

    private static bool Calculate(bool a, bool b, Op op) => op switch
    {
        Op.AND => a & b,
        Op.OR => a | b,
        Op.XOR => a ^ b,

        _ => throw new NotImplementedException(),
    };

    private static ulong BitArray2UInt64(IEnumerable<bool> arr)
        => arr.WithIndex().Where(b => b.item).Aggregate(0UL, (acc, b) => acc |= 1UL << b.index);

    private static ulong GetNum(Dictionary<string, bool> memory, char num)
        => BitArray2UInt64(memory.Where(kv => kv.Key.StartsWith(num))
            .OrderBy(kv => kv.Key)
            .Select(kv => kv.Value));

    #region Regex

    [GeneratedRegex(@"(?<name>.*?)\:\s+(?<value>\d)", RegexOptions.Compiled)]
    private static partial Regex ValueRegex();
    [GeneratedRegex(@"(?<in1>\S*)\s+(?<op>\S*)\s+(?<in2>\S*)\s+->\s+(?<out>\S*)", RegexOptions.Compiled)]
    private static partial Regex OperationsRegex();

    #endregion

    #endregion
}
