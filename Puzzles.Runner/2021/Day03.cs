namespace Puzzles.Runner._2021;

using Map = Map2<char>;

[Puzzle("Binary Diagnostic", 3, 2021)]
public class Day03(ILinesInputReader input) : IPuzzleSolver
{
    private enum LSR { Ox, CO2 };
    private Map _map = Map.Null;

    public void Init()
    {
        _map = new Map
        (
            data: [.. input.Lines.OrderByDescending(line => line)
                .SelectMany(line => line.AsEnumerable())],
            columns: input.Lines[0].Length
        );
    }

    public string SolvePart1()
    {
        var half = input.Lines.Length / 2;
        var mask = (1 << _map.Columns) - 1;

        var gamma = Enumerable.Range(0, _map.Columns)
            .Select(i => _map.Column(i).Count(ch => ch == '1'))
            .Aggregate(0, (acc, v) => acc = (acc << 1) | (v > half ? 1 : 0));

        var epsilon = (~gamma) & mask;

        return (gamma * epsilon).ToString();
    }

    public string SolvePart2()
    {
        var ox = CalculateLSR(_map, LSR.Ox);
        var co2 = CalculateLSR(_map, LSR.CO2);

        return (ox * co2).ToString();
    }

    private static long CalculateLSR(Map map, LSR lsr)
    {
        var less = lsr == LSR.CO2;

        var start = 0;
        var end = map.Rows;

        for (int c = 0; end - start > 1 && c < map.Columns; c++)
            (start, end) = CalculateLSR(map, c, start, end, less);

        return BitStringToLong(map.Row(start));
    }

    private static long BitStringToLong(IEnumerable<char> bits)
        => bits.Aggregate(0L, (acc, c) => (acc << 1) | (c == '1' ? 1L : 0L));

    private static (int start, int end) CalculateLSR(Map map, int column, int start, int end, bool less)
    {
        var half = (int)Math.Ceiling((end - start) / 2.0);
        var count = 0;
        while (map[column, start + count] == '1')
            count++;

        return (count >= half) ^ less ? (start, start + count) : (start + count, end);
    }
}
