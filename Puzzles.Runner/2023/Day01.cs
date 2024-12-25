using System.Text.RegularExpressions;

namespace Puzzles.Runner._2023;

[Puzzle("Trebuchet?!", 1, 2023)]
public partial class Day01(ILinesInputReader input) : IPuzzleSolver
{
    public string SolvePart1()
        => input.Lines.AsParallel().Sum(line => MergeDigits(line, @"\d")).ToString();

    public string SolvePart2()
        => input.Lines.AsParallel().Sum(line => MergeDigits(line, @"\d|one|two|three|four|five|six|seven|eight|nine")).ToString();

    private static int MergeDigits(string str, string regex)
    {
        var first = Regex.Match(str, regex).Value;
        var last = Regex.Match(str, regex, RegexOptions.RightToLeft).Value;

        return Str2Int(first) * 10 + Str2Int(last);
    }

    private static int Str2Int(string str) => str switch
    {
        "one" => 1,
        "two" => 2,
        "three" => 3,
        "four" => 4,
        "five" => 5,
        "six" => 6,
        "seven" => 7,
        "eight" => 8,
        "nine" => 9,
        var d => Convert.ToInt32(d)
    };
}
