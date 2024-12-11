namespace Puzzles.Runner._2024;

[Puzzle("Plutonian Pebbles", 11, 2024)]
public class Day11(ILinesInputReader input) : IPuzzleSolver
{
    private ulong[] _numbers = [];
    private readonly Dictionary<(ulong, int), ulong> _cache = [];

    public void Init()
        =>_numbers = input.GetTokens(" ", Convert.ToUInt64).First();

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
        if (_cache.ContainsKey((num, count)))
            return _cache[(num, count)];

        ulong result;

        if (count == 0)
        {
            result = 1;
        }
        else if (num == 0)
        {
            result = Blink(1, count - 1);
        }
        else
        {
            var digits = (ulong)(Math.Log10(num) + 1);
            if (digits % 2 == 0)
            {
                var div = (ulong)Math.Pow(10, digits / 2);
                result = Blink(num / div, count - 1) + Blink(num % div, count - 1);
            }
            else
            {
                result = Blink(num * 2024, count - 1);
            }
        }

        _cache.Add((num, count), result);
        return result;
    }
}
