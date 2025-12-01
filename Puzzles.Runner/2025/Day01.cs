namespace Puzzles.Runner._2025;

[Puzzle("Secret Entrance", 1, 2025)]
public partial class Day01(ILinesInputReader input) : IPuzzleSolver
{
    private const int MOD = 100;

    public string SolvePart1()
        => Sequence().Count(d => d.deal == 0).ToString();

    public string SolvePart2()
        => Sequence().Sum(d => d.zeros).ToString();

    private IEnumerable<(int deal, int zeros)> Sequence()
    {
        var deal = 50;

        foreach (var instr in input.Lines)
        {
            var nd = deal + (instr[0] == 'L' ? -1 : 1) * Int32.Parse(instr.AsSpan()[1..]);
            int zeros = Math.Abs(nd) / MOD + (nd <= 0 && deal > 0 ? 1 : 0);

            deal = AOC.Mod(nd, MOD);
            yield return (deal, zeros);
        }
    }
}
