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
        var num = 0L;
        for(int i = 0, left = 0; i < size; i++)
        {
            var right = digits.Length - (size - i - 1) ;
            var midx = digits[left..right].IndexOfMax();

            num = num * 10 + digits[midx + left];
            left = midx + left + 1;
        }

        return num;
    }
}
