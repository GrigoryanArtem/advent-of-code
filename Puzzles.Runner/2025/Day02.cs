namespace Puzzles.Runner._2025;

[Puzzle("Gift Shop", 2, 2025)]
public partial class Day02(IFullInputReader input) : IPuzzleSolver
{
    private record Range(ulong From, ulong To);

    private Range[] _ranges = [];
    private ulong[] _patterns = [];

    public void Init()
    {        
        _ranges = [..input.Text.Split(',').Select(rng =>
        {
            var tokens = rng.Split('-', 2);
            return new Range(UInt64.Parse(tokens[0]), UInt64.Parse(tokens[1]));
        })];

        var patterns = new HashSet<ulong>();
        Enumerable.Range(2, 10).ForEach(i => GeneratePatterns(i, patterns));
        _patterns = [.. patterns.Distinct().OrderBy(x => x)];
    }

    public string SolvePart1()
        => CalculateSum(_patterns.AsSpan(), IsPalindrome).ToString();

    public string SolvePart2()
        => CalculateSum(_patterns.AsSpan(), _ => true).ToString();    

    private ulong CalculateSum(Span<ulong> patterns, Predicate<ulong> predicate)
    {        
        var sum = 0UL;

        foreach (var r in _ranges)
        {
            var idx = patterns.BinarySearch(r.From);

            if (idx < 0)
                idx = Math.Abs(idx) - 1;

            while (patterns[idx] <= r.To)
            {
                var pattern = patterns[idx++];
                if (predicate(pattern))
                    sum += pattern;
            }
        }

        return sum;
    }

    private static bool IsPalindrome(ulong num)
    {
        var digits = AOC.GetDigits(num);
        var divider = AOC.DigitsDividers[digits / 2];

        return digits > 1 && num / divider == num % divider;
    }

    private static void GeneratePatterns(int digits, HashSet<ulong> dest)
    {
        for (int block = 1; block <= digits / 2; block++)
        {
            if (digits % block != 0)
                continue;

            var start = AOC.DigitsDividers[block - 1];
            var end = AOC.DigitsDividers[block];

            for (var @base = start; @base < end; @base++)
            {
                var result = 0UL;
                for (int rep = 0; rep < digits / block; rep++)
                    result = result * AOC.DigitsDividers[block] + @base;

                dest.Add(result);
            }
        }
    }
}
