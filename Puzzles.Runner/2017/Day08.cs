using System.Text.RegularExpressions;

namespace Puzzles.Runner._2017;

[Puzzle("I Heard You Like Registers", 8, 2017)]
public partial class Day08(ILinesInputReader input) : IPuzzleSolver
{
    private record Op(string Reg, int Action, int Delta, string Condition);

    private Op[] _operations = [];

    public void Init()
    {
        _operations = input.Convert(line =>
        {
            var match = ParseRegex().Match(line);
            return new Op
            (
                match.Groups["reg"].Value,
                match.Groups["op"].Value == "inc" ? 1 : -1,
                Convert.ToInt32(match.Groups["delta"].Value),
                match.Groups["condition"].Value
            );
        });
    }

    [GeneratedRegex(@"(?<reg>\w+)\s+(?<op>(inc|dec))\s+(?<delta>-?\d+)\s+if\s+(?<condition>.*)")]
    private static partial Regex ParseRegex();

    public string SolvePart1()
    {
        var mem = new Dictionary<string, int>();
        foreach (var op in _operations)
        {
            var condition = IsConditionTrue(op.Condition, mem);

            if (!condition)
                continue;

            mem.TryAdd(op.Reg, 0);
            mem[op.Reg] += op.Action * op.Delta;
        }

        return mem.Values.Max().ToString();
    }

    public string SolvePart2()
    {
        var mem = new Dictionary<string, int>();
        var max = 0;

        foreach (var op in _operations)
        {
            var condition = IsConditionTrue(op.Condition, mem);

            if (!condition)
                continue;

            mem.TryAdd(op.Reg, 0);
            mem[op.Reg] += op.Action * op.Delta;

            max = Math.Max(max, mem[op.Reg]);
        }

        return max.ToString();
    }

    private bool IsConditionTrue(string condition, Dictionary<string, int> mem)
    {
        var tokens = condition.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var a = GetValue(tokens[0], mem);
        var b = GetValue(tokens[2], mem);

        return tokens[1] switch
        {
            "==" => a == b,
            "<" => a < b,
            ">" => a > b,
            "<=" => a <= b,
            ">=" => a >= b,
            "!=" => a != b,

            _ => throw new NotSupportedException()
        };
    }

    private int GetValue(string token, Dictionary<string, int> mem)
        => Int32.TryParse(token, out var val)
            ? val
            : mem.GetOrAdd(token, 0);
}
