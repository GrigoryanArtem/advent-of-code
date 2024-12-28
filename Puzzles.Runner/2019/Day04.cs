namespace Puzzles.Runner._2019;

[Puzzle("Crossed Wires", 4, 2019)]
public class Day04(IFullInputReader input) : IPuzzleSolver
{
    private int _from = 0;
    private int _to = 0;

    public void Init()
    {
        var split = input.Text.Split('-').Select(s => Convert.ToInt32(s)).ToArray();
        (_from, _to) = (split[0], split[1]);
    }

    public string SolvePart1()
        => Enumerable.Range(_from, _to - _from).AsParallel().Count(n => IsValid(n, false)).ToString();

    public string SolvePart2()
        => Enumerable.Range(_from, _to - _from).AsParallel().Count(n => IsValid(n, true)).ToString();

    public static bool IsValid(int num, bool adjacentMatching)
    {
        var hasDoubles = false;

        int gCount = 1;
        int prev = num % 10;

        while ((num /= 10) > 0)
        {
            var cur = num % 10;

            if (cur > prev)
                return false;

            hasDoubles |= cur != prev && (adjacentMatching ? gCount == 2 : gCount >= 2);
            gCount = (cur == prev) ? gCount + 1 : 1;

            prev = cur;
        }

        return hasDoubles || (adjacentMatching ? gCount == 2 : gCount >= 2);
    }
}
