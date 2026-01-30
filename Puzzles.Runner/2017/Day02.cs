namespace Puzzles.Runner._2017;

[Puzzle("Corruption Checksum", 2, 2017)]
public class Day02(ILinesInputReader input) : IPuzzleSolver
{
    private int[][] _lines = [];

    public void Init()
        => _lines = [..input.Lines.Select(line => line
            .Split([' ', '\t'], options: StringSplitOptions.RemoveEmptyEntries)
            .Select(Int32.Parse).ToArray())];

    public string SolvePart1() => _lines.Sum(line =>
    {
        var (min, max) = line.MinMax(x => x);
        return max - min;
    }).ToString();

    public string SolvePart2()
    {
        var sum = 0;

        foreach (var line in _lines)
            for (int i = 0; i < line.Length; i++)
                for (int k = 0; k < line.Length; k++)
                    if (line[i] > line[k] && line[i] % line[k] == 0)
                        sum += line[i] / line[k];

        return sum.ToString();
    }
}
