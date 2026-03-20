namespace Puzzles.Runner._2023;

[Puzzle("Scratchcards ", 4, 2023)]
public partial class Day04(ILinesInputReader input) : IPuzzleSolver
{
    public string SolvePart1()
        => input.Lines.Sum(line => AOC.Sqr(GetWinningCards(line))).ToString();

    public string SolvePart2()
    {
        Span<int> counts = stackalloc int[input.Lines.Length];
        counts.Fill(1);
        var sum = 0L;

        foreach (var (line, idx) in input.Lines.WithIndex())
        {
            var numberOfCards = GetWinningCards(line);
            var count = counts[idx];

            for (int i = 1; i <= numberOfCards && (idx + i) < counts.Length; i++)
                counts[idx + i] += count;

            sum += count;
        }

        return sum.ToString();
    }

    public static int GetWinningCards(string str)
    {        
        var cidx = str.IndexOf(':');
        var nidx = str.IndexOf('|');

        var winning = Parse.StringToNumbers(str, cidx + 1, nidx).ToHashSet();        
        return Parse.StringToNumbers(str, nidx + 1, str.Length).Count(winning.Contains);
    }
}
