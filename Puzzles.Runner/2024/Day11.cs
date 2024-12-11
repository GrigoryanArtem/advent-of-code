namespace Puzzles.Runner._2024;

[Puzzle("Plutonian Pebbles", 11, 2024)]
public class Day11(ILinesInputReader input) : IPuzzleSolver
{
    private ulong[] _numbers = [];
    private readonly Dictionary<(ulong, int), ulong> _cache = [];

    public void Init()
        => _numbers = input.GetTokens(" ", Convert.ToUInt64).First();

    public string SolvePart1()
    { 
        _cache.Clear();
        return _numbers.UInt64Sum(n => Blink(n, 25)).ToString();
    }

    public string SolvePart2()
    {
        _cache.Clear();
        return _numbers.UInt64Sum(n => Blink(n, 75)).ToString();
    }

    private ulong Blink(ulong num, int count)
    {
        var tuple = (num, count);

        if (_cache.TryGetValue(tuple, out ulong value))
            return value;

        return _cache.AddAndReturn(tuple, tuple switch
        {
            (_, 0) => 1UL,
            (0, _) => Blink(1, count - 1),
            _ => Rule3(num, count)
        });
    }

    private ulong Rule3(ulong num, int count)
    {
        var digits = AOC.GetDigits(num);

        if (digits % 2 != 0)        
            return Blink(num * 2024, count - 1);

        var (left, right) = AOC.SplitUInt64(num, digits / 2);
        return Blink(left, count - 1) + Blink(right, count - 1);
    }
}
