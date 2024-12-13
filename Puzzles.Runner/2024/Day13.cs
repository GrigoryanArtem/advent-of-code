using System.Text.RegularExpressions;

namespace Puzzles.Runner._2024;

[Puzzle("Claw Contraption", 13, 2024)]
public partial class Day13(IFullInputReader input) : IPuzzleSolver
{
    private record ClawMachine(long[] Mat, long[] Sol);

    #region Constants

    private readonly string[] MatTemplate = ["ax", "bx", "ay", "by"];
    private readonly string[] SolTemplate = ["px", "py"];

    private const long P2_K = 10000000000000;

    #endregion

    private ClawMachine[] _machines = [];

    public void Init() => _machines = PuzzleRegex()
        .Matches(input.Text)
        .Select(m => new ClawMachine
        (
            Mat: [.. MatTemplate.Select(t => M2L(m, t))],
            Sol: [.. SolTemplate.Select(t => M2L(m, t))]
        )).ToArray();

    public string SolvePart1()
        => _machines.Sum(m => CalculatePrice(m)).ToString();

    public string SolvePart2()
        => _machines.Sum(m => CalculatePrice(m, P2_K)).ToString();

    #region Private methods

    private static long CalculatePrice(ClawMachine machine, long solDiff = 0L)
    {
        var (mat, sol) = (machine.Mat, machine.Sol);
        var det = (mat[0] * mat[3]) - (mat[1] * mat[2]);
        if (det == 0L)
            return 0L;

        var a = mat[3] * (sol[0] + solDiff) + -mat[1] * (sol[1] + solDiff);
        var b = -mat[2] * (sol[0] + solDiff) + mat[0] * (sol[1] + solDiff);

        return a % det == 0L && b % det == 0L ? a / det * 3L + (b / det) : 0L;
    }

    private static long M2L(Match match, string group)
        => Convert.ToInt64(match.Groups[group].Value);

    [GeneratedRegex(@"Button A: X\+(?<ax>\d+), Y\+(?<ay>\d+)\s+Button B: X\+(?<bx>\d+), Y\+(?<by>\d+)\s+Prize: X\=(?<px>\d+), Y\=(?<py>\d+)", RegexOptions.Compiled)]
    private static partial Regex PuzzleRegex();

    #endregion
}
