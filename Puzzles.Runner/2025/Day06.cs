namespace Puzzles.Runner._2025;

using Mat64 = Mat2<ulong>;

[Puzzle("Trash Compactor", 6, 2025)]
public class Day06(ILinesInputReader input) : IPuzzleSolver
{
    public string SolvePart1()
    {
        var _operations = input.Lines.Last()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.First())
            .ToArray();

        var mat = new Mat64([.. input.Lines[..^1].SelectMany(line => line
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(UInt32.Parse))], _operations.Length);

        return _operations.WithIndex()
            .UInt64Sum(d => d.item == '*'
                ? mat.Column(d.index).UInt64Mul(a => a)
                : mat.Column(d.index).UInt64Sum(a => a))
            .ToString();
    }

    public string SolvePart2()
        => CalculateSum(input.Lines[..^1], input.Lines.Last()).ToString();

    private static ulong CalculateSum(string[] numbers, string operations)
    {
        var func = Add;

        var sum = 0UL;
        var acc = 0UL;

        for (int i = 0; i < operations.Length; i++)
        {
            if (operations[i] != ' ')
            {
                sum += acc;

                func = operations[i] == '*' ? Mul : Add;
                acc = operations[i] == '*' ? 1UL : 0UL;
            }

            var num = 0U;
            foreach (var d in numbers)
                num = d[i] == ' ' ? num : num * 10 + C2D(d[i]);

            acc = func(acc, num);
        }

        return sum + acc;
    }

    private static uint C2D(char ch)
        => (uint)ch - '0';

    private static ulong Add(ulong acc, uint val)
        => acc + val;

    private static ulong Mul(ulong acc, uint val)
        => acc * Math.Max(val, 1U);
}
