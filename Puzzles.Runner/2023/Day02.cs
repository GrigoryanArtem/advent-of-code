using System.Text.RegularExpressions;

namespace Puzzles.Runner._2023;

[Puzzle("Cube Conundrum", 2, 2023)]
public partial class Day02(ILinesInputReader input) : IPuzzleSolver
{
    private record Game(int Red, int Green, int Blue)
    {
        public static Game Parse(string game) => new
        (
            Red: ParseColorValue(game, "red"),
            Green: ParseColorValue(game, "green"),
            Blue: ParseColorValue(game, "blue")
        );

        private static int ParseColorValue(string game, string color)
            => Regex.Matches(game, $"(?<value>\\d+)\\s+{color}")
                .Select(m => Convert.ToInt32(m.Groups["value"].Value))
                .Max();
    }

    public string SolvePart1()
    {
        var restriction = new Game
        (
            Red: 12,
            Green:  13,
            Blue: 14
        );

        return input.Lines
            .AsParallel()
            .Select(Game.Parse)
            .WithIndex()
            .Sum(d => d.item.Red <= restriction.Red &&
                d.item.Green <= restriction.Green &&
                d.item.Blue <= restriction.Blue ? d.index + 1 : 0)
            .ToString();
    }

    public string SolvePart2()
        => input.Lines
            .AsParallel()
            .Select(Game.Parse)
            .Sum(g => g.Red * g.Green * g.Blue)
            .ToString();
}
