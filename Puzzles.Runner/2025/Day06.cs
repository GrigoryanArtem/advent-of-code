namespace Puzzles.Runner._2025;

[Puzzle("Trash Compactor", 6, 2025)]
public class Day06(ILinesInputReader input) : IPuzzleSolver
{
    public string SolvePart1()
        => CalculateSumH(input.Lines[..^1], input.Lines.Last()).ToString();

    public string SolvePart2()
        => CalculateSumV(input.Lines[..^1], input.Lines.Last()).ToString();

    private static ulong CalculateSumH(string[] numbers, string operations)
    {
        var func = Add;

        var sum = 0UL;
        var acc = 0UL;

        var indecies = new int[numbers.Length];

        for (int i = 0; i < operations.Length; i++)
        {
            if (operations[i] == ' ')
                continue;

            func = operations[i] == '*' ? Mul : Add;
            acc = operations[i] == '*' ? 1UL : 0UL;

            for (int nidx = 0; nidx < indecies.Length; nidx++)
            {
                var num = 0U;
                for (; indecies[nidx] < numbers[nidx].Length && numbers[nidx][indecies[nidx]] == ' '; indecies[nidx]++);
                for (; indecies[nidx] < numbers[nidx].Length && numbers[nidx][indecies[nidx]] != ' '; indecies[nidx]++)
                    num = num * 10 + C2D(numbers[nidx][indecies[nidx]]);

                acc = func(acc, num);
            }

            sum += acc;
        }

        return sum;
    }

    private static ulong CalculateSumV(string[] numbers, string operations)
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
