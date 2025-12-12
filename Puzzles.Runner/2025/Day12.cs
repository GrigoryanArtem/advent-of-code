using System.Text.RegularExpressions;

namespace Puzzles.Runner._2025;

[Puzzle("Christmas Tree Farm", 12, 2025)]
public partial class Day12(IFullInputReader input) : IPuzzleSolver
{
    private record Region(int Width, int Height, int[] Shapes);

    private Region[] _regions = [];
    private int[] _shapes = [];

    public void Init()
    {
        var tokens = input.Text.Split("\r\n\r\n");

        _shapes = [..tokens[..^1].Select(x => x.Count(ch => ch == '#'))];
        _regions = [.. tokens.Last().Split("\r\n").Select(line =>
        {
            var numbers = NumRegex().Matches(line).Select(m => Int32.Parse(m.Value)).ToArray();

            return new Region
            (
                Width: numbers[0],
                Height: numbers[1],
                Shapes: numbers[2..]
            );
        })];
    }

    public string SolvePart1()
        => _regions
            .Count(r => r.Shapes.Zip(_shapes, (rs, s) => rs * s)
                .Sum() <= (r.Width * r.Height))
            .ToString();

    [GeneratedRegex(@"\d+")]
    private static partial Regex NumRegex();
}
