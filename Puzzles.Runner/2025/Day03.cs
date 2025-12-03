namespace Puzzles.Runner._2025;

[Puzzle("Lobby", 3, 2025)]
public partial class Day03(ILinesInputReader input) : IPuzzleSolver
{
    private int[][] _data = [];

    public void Init()
        => _data = [.. input.Lines.Select(line => line.AsEnumerable().Select(ch => ch - '0').ToArray())];

    public string SolvePart1()
        => _data
            .Sum(x => FindLargestNumber(x, 2))
            .ToString();

    public string SolvePart2()
        => _data
            .Sum(x => FindLargestNumber(x, 12))
            .ToString();

    public static long FindLargestNumber(Span<int> digits, int size)
    {
        var ids = Enumerable.Range(digits.Length - size, size).ToArray();

        ids[0] = digits[0..(ids[0] + 1)].IndexOfMax();
        for (int i = 1; i < ids.Length; i++)
        {
            var start = ids[i - 1] + 1;
            var end = ids[i] + 1;

            ids[i] = start + digits[start..end].IndexOfMax();
        }

        return BuildNumber(digits, ids);
    }

    private static long BuildNumber(Span<int> digits, Span<int> ids)
    {
        var num = 0L;

        foreach(var i in ids)
            num = num * 10 + digits[i];

        return num;
    }
}
