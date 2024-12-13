using System.Text.RegularExpressions;

namespace Puzzles.Runner._2024;

[Puzzle("Claw Contraption", 13, 2024)]
public partial class Day13(IFullInputReader input) : IPuzzleSolver
{
    private record ClawMachine(decimal[] Mat, decimal[] Sol);
    private const decimal P2_K = 10000000000000;
    private ClawMachine[] _machines = [];

    public void Init() => _machines = PuzzleRegex()
        .Matches(input.Text)
        .Select(m => new ClawMachine
        (
            Mat:
            [
                G2D(m, "ax"), G2D(m, "bx"),
                G2D(m, "ay"), G2D(m, "by"),
            ],
            Sol:
            [
                G2D(m, "px"),
                G2D(m, "py")
            ]
        )).ToArray();

    public string SolvePart1()
        => _machines.Where(p => Det(p.Mat) != 0)
            .Select(p => MatByVec(Inv(p.Mat), p.Sol))
            .Sum(r => IsValid(r) ? Math.Round(r[0] * 3 + r[1]) : 0m)
            .ToString("f0");

    public string SolvePart2()
        => _machines.Where(p => Det(p.Mat) != 0)
            .Select(p => MatByVec(Inv(p.Mat), [..p.Sol.Select(s => s + P2_K)]))
            .Sum(r => IsValid(r) ? Math.Round(r[0] * 3 + r[1]) : 0m)
            .ToString("f0");

    #region Private methods

    // Determinant
    private static decimal Det(decimal[] mat)
        => (mat[0] * mat[3]) - (mat[1] * mat[2]);

    // Inverse
    private static decimal[] Inv(decimal[] mat)
    {
        var invDet = 1 / Det(mat);
        return [invDet * mat[3], -invDet * mat[1], -invDet * mat[2], invDet * mat[0]];
    }

    // Multiply matrix by vector
    private static decimal[] MatByVec(decimal[] mat, decimal[] vec) => 
    [
        mat[0] * vec[0] + mat[1] * vec[1],
        mat[2] * vec[0] + mat[3] * vec[1] 
    ];

    private static decimal G2D(Match match, string group)
        => Convert.ToDecimal(match.Groups[group].Value);

    private static bool IsValid(decimal[] vec)
        => vec.All(v => Math.Abs(v - Math.Round(v)) < 1e-7m);

    [GeneratedRegex(@"Button A: X\+(?<ax>\d+), Y\+(?<ay>\d+)\s+Button B: X\+(?<bx>\d+), Y\+(?<by>\d+)\s+Prize: X\=(?<px>\d+), Y\=(?<py>\d+)", RegexOptions.Compiled)]
    private static partial Regex PuzzleRegex();

    #endregion
}
