namespace Puzzles.Runner._2021;

[Puzzle("Binary Diagnostic", 3, 2021)]
public class Day03(ILinesInputReader input) : IPuzzleSolver
{
    private enum LSR { Ox, CO2 };

    public string SolvePart1()
    {
        var buffer = new int[input.Lines[0].Length];
        var half = input.Lines.Length / 2;

        input.Lines.ForEach(line => line.WithIndex()
            .ForEach(d => buffer[d.index] += d.item == '1' ? 1 : 0));

        var gamaValue = buffer.Aggregate(0, (acc, v) => acc = (acc << 1) | (v > half ? 1 : 0));
        var mask = (1 << buffer.Length) - 1;
        var epsilonValue = (~gamaValue) & mask;

        return (gamaValue * epsilonValue).ToString();
    }

    public string SolvePart2()
    {        
        var ox = CalculateLSR(LSR.Ox);
        var co2 = CalculateLSR(LSR.CO2);

        return (ox * co2).ToString();
    }

    private long CalculateLSR(LSR lsr)
    {
        var less = lsr == LSR.CO2;
        var set = Enumerable.Range(0, input.Lines.Length).ToHashSet();

        for (int b = 0; b < input.Lines[0].Length; b++)
        {
            if (set.Count == 1)
                break;

            var half = (int)Math.Ceiling(set.Count / 2.0);
            var count = input.Lines.WithIndex().Where(d => set.Contains(d.index)).Count(d => d.item[b] == '1');

            var target = (count >= half) ^ less ? '1' : '0';
            set.IntersectWith(input.Lines.WithIndex()
                .Where(line => line.item[b] == target)
                .Select(d => d.index));
        }

        var idx = set.Single();
        return Convert.ToInt64(input.Lines[idx], 2);
    }
}
