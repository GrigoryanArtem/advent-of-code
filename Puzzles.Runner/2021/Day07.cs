namespace Puzzles.Runner._2021;


[Puzzle("The Treachery of Whales", 7, 2021)]
public class Day07(IFullInputReader input) : IPuzzleSolver
{
    private int[] _positions = [];

    public void Init()
        => _positions = [.. input.Text.Split(',').Select(Int32.Parse)];

    public string SolvePart1()
    {
        var median = AOC.Median(_positions);
        return _positions.Sum(v => Math.Abs(v - median)).ToString();
    }

    public string SolvePart2()
    {
        var avg = _positions.Average();
        var (favg, cavg) = ((int)Math.Floor(avg), (int)Math.Ceiling(avg));

        var min = Math.Min
        (
            _positions.Sum(v => APS(Math.Abs(v - favg))),
            _positions.Sum(v => APS(Math.Abs(v - cavg)))
        );

        return min.ToString();
    }

    private static int APS(int n)
        => (n * (1 + n)) / 2;
}
