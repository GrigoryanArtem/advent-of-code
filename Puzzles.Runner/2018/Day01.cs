namespace Puzzles.Runner._2018;

[Puzzle("Chronal Calibration", 1, 2018)]
public class Day01(ILinesInputReader input) : IPuzzleSolver
{
    private int[] _input = [];

    public void Init()
        => _input = [.. input.Lines.Select(Int32.Parse)];

    public string SolvePart1()
        => _input.Sum().ToString();

    public string SolvePart2()
    {
        HashSet<int> set = [];

        var current = 0;
        for (int i = 0; !set.Contains(current); i = AOC.Mod(i + 1, _input.Length))
        {
            set.Add(current);
            current += _input[i];
        }

        return current.ToString();
    }
}
