namespace Puzzles.Runner._2025;

[Puzzle("Gift Shop", 2, 2025)]
public partial class Day02(IFullInputReader input) : IPuzzleSolver
{
    private record Range(ulong From, ulong To);

    private Range[] _ranges = [];

    public void Init()
    {
        _ranges = [..input.Text.Split(',').Select(rng =>
        {
            var tokens = rng.Split('-', 2);
            return new Range(UInt64.Parse(tokens[0]), UInt64.Parse(tokens[1]));
        })];
    }

    public string SolvePart1()
    {
        var sum = 0UL;

        foreach(var r in _ranges)
            for (var num = r.From; num <= r.To; num++)
                if (IsSymmetric(num))
                    sum += num;

        return sum.ToString();
    }

    public string SolvePart2()
    {
        var sum = 0UL;

        foreach (var r in _ranges)
            for (var num = r.From; num <= r.To; num++)
                if (HasPattern(num))
                    sum += num;

        return sum.ToString();
    }

    private static bool IsSymmetric(ulong num)
    {
        var digits = AOC.GetDigits(num);
        var divider = AOC.DigitsDividers[digits / 2];

        return digits > 1 && num / divider == num % divider;
    }

    private static bool HasPattern(ulong num)
    {
        var digits = AOC.GetDigits(num);

        for(int i = 1; i <= digits / 2; i++)
            if (HasPattern(num, digits, i))
                return true;

        return false;
    }

    private static bool HasPattern(ulong num, int size, int digits)
    {
        if (size % digits != 0)
            return false;

        var hasPattern = true;

        var target = num % AOC.DigitsDividers[digits];
        for (int k = 1; k < size / digits; k++)
            hasPattern &= target == num / AOC.DigitsDividers[digits * k] % AOC.DigitsDividers[digits];

        return hasPattern;
    }
}
