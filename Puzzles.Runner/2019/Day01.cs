namespace Puzzles.Runner._2019;

[Puzzle("The Tyranny of the Rocket Equation", 1, 2019)]
public class Day01(ILinesInputReader input) : IPuzzleSolver
{
    private long[] _numbers = [];

    public void Init()
        => _numbers = input.Lines.Select(s => Convert.ToInt64(s)).ToArray();

    public string SolvePart1()
        => _numbers.Sum(Fuel).ToString();

    public string SolvePart2()
        => _numbers.Sum(FullFuel).ToString();

    private long FullFuel(long num)
    {
        var fuel = Fuel(num);
        return fuel > 0 ? fuel + FullFuel(fuel) : 0;
    }

    private long Fuel(long num)
        => Math.Max(0, num / 3 - 2);
}
