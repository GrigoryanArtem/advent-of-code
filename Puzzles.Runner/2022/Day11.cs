using System.Text.RegularExpressions;

namespace Puzzles.Runner._2022;

[Puzzle("Monkey in the Middle", 11, 2022)]
public partial class Day11(IFullInputReader input) : IPuzzleSolver
{    
    private class Monkey
    {
        public required int Id { get; init; }
        public required Func<long, long> Operation { get; init; }
        public required Queue<long> Items { get; init; }
        public required int TestDivisible { get; init; }
        public required int TrueBranch { get; init; }
        public required int FalseBranch { get; init; }
        public int InspeactionCount { get; private set; }

        public bool TryInspect(Func<Monkey, long, long> wop, out int target, out long updated)
        {
            updated = target = 0;
            var success = Items.TryDequeue(out var item);

            if (success)
            { 
                updated = wop(this, Operation(item));
                target = updated % TestDivisible == 0
                    ? TrueBranch
                    : FalseBranch;
                InspeactionCount++;
            }

            return success;
        }
    }

    public string SolvePart1()
    {
        var monkeys = Parse();

        Loop(monkeys, (_, x) => x / 3, 20);

        return CalculateTwoBest(monkeys).ToString();
    }

    public string SolvePart2()
    {
        var monkeys = Parse();

        var mod = monkeys.Aggregate(1, (mod, monkey) => mod * monkey.TestDivisible);
        Loop(monkeys, (m, x) => x % mod, 10000);

        return CalculateTwoBest(monkeys).ToString();
    }

    private static long CalculateTwoBest(Monkey[] monkeys)
        => monkeys.Select(m => m.InspeactionCount)
            .OrderByDescending(x => x)
            .Take(2)
            .Aggregate(1L, (acc, x) => acc * x);

    private Monkey[] Parse() => [.. MonkeyRegex().Matches(input.Text).Select(m =>
    {
        return new Monkey()
        {
            Id = Convert.ToInt32(m.Groups["id"].Value),

            Operation = x => Operation(x, m.Groups["operation"].Value),

            Items = new(m.Groups["items"].Value.Split(',').Select(v => Convert.ToInt64(v.Trim()))),
            TestDivisible= Convert.ToInt32(m.Groups["div"].Value),

            TrueBranch = Convert.ToInt32(m.Groups["true_id"].Value),
            FalseBranch = Convert.ToInt32(m.Groups["false_id"].Value),
        };
    })];

    private static void Loop(Monkey[] monkeys, Func<Monkey, long, long> wop, int count)
    {
        for (int i = 0; i < count; i++)
            foreach (var (monkey, idx) in monkeys.WithIndex())
                while (monkey.TryInspect(wop, out var target, out var updated))
                    monkeys[target].Items.Enqueue(updated);
    }

    private static long Operation(long x, string expresion)
    {
        var exp = expresion.Replace("old", x.ToString());
        var tokens = exp.Split(' ', options: StringSplitOptions.RemoveEmptyEntries);
        var (a, op, b) = (Convert.ToInt64(tokens[0]), tokens[1], Convert.ToInt64(tokens[2]));

        return op switch
        {
            "+" => a + b,
            "-" => a - b,

            "*" => a * b,
            "/" => a / b,

            _ => throw new NotImplementedException()
        };
    }

    [GeneratedRegex(@"Monkey\s+(?<id>\d+)\:\s+Starting items:\s+(?<items>[\d, ]+)\s+Operation:\s+new = (?<operation>.*?)\s+Test:\s+divisible by\s+(?<div>\d+)\s+If true: throw to monkey\s+(?<true_id>\d+)\s+If false: throw to monkey\s+(?<false_id>\d+)", options: RegexOptions.Singleline)]
    private static partial Regex MonkeyRegex();
}
