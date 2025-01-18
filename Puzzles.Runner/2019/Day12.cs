using System.Text.RegularExpressions;

namespace Puzzles.Runner._2019;

[Puzzle("The N-Body Problem", 12, 2019)]
public partial class Day12(ILinesInputReader input) : IPuzzleSolver
{
    private const int DEMENSIONS = 3;

    private int[][] _moons = [];

    public void Init()
        => _moons = input.Lines.Select(line => M2V3(MoonRegex().Match(line))).ToArray();

    public string SolvePart1()
    {
        var (moons, velocities) = Simulate().ElementAt(999);
        return Energy(moons, velocities).ToString();
    }

    private IEnumerable<(int[][] moons, int[][] velocities)> Simulate()
    {
        var moons = _moons.Select(m => m.ToArray()).ToArray();
        var velocities = new int[moons.Length][];

        for (int i = 0; i < moons.Length; i++)
            velocities[i] = new int[DEMENSIONS];

        while (true)
        {
            for (int i = 0; i < moons.Length; i++)
                for (int k = 0; k < moons.Length; k++)
                    for (int d = 0; d < DEMENSIONS; d++)
                        velocities[i][d] += Math.Sign(moons[k][d] - moons[i][d]);

            for (int i = 0; i < moons.Length; i++)
                for (int d = 0; d < DEMENSIONS; d++)
                    moons[i][d] += velocities[i][d];

            yield return (moons, velocities);
        }
    }
    private static int Energy(int[][] moons, int[][] velocities)
        => moons.Zip(velocities, (m, v) => Value(m) * Value(v)).Sum();

    private static int Value(int[] vec)
        => vec.Sum(Math.Abs);

    private static int[] M2V3(Match match)
        => [M2I32(match, "x"), M2I32(match, "y"), M2I32(match, "z")];

    private static int M2I32(Match match, string group)
        => Convert.ToInt32(match.Groups[group].Value);

    [GeneratedRegex(@"<x=(?<x>-?\d+),\s+y=(?<y>-?\d+),\s+z=(?<z>-?\d+)>")]
    private static partial Regex MoonRegex();
}
