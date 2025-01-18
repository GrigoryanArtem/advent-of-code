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

    public string SolvePart2()
    {
        ulong?[] periods = new ulong?[DEMENSIONS];
        var hashes = Enumerable.Range(0, DEMENSIONS)
            .Select(_ => new HashSet<(int, int, int, int, int, int, int, int)>())
            .ToArray();

        foreach (var (moons, velocities) in Simulate())
        {
            if (periods.All(p => p.HasValue))
                break;

            for (int d = 0; d < periods.Length; d++)
            {
                if (periods[d].HasValue)
                    continue;
                
                var hash = (moons[0][d], moons[1][d], moons[2][d], moons[3][d], velocities[0][d], velocities[1][d], velocities[2][d], velocities[3][d]);                
                if (hashes[d].Contains(hash))
                    periods[d] = (ulong)hashes[d].Count;
                else
                    hashes[d].Add(hash);
            }
        }

        return AOC.LCM(AOC.LCM(periods[0].Value, periods[1].Value), periods[2].Value).ToString();
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

    private static int Hash(IEnumerable<int> data)
    {
        int hash = 127; // Start with a prime number
        foreach (var num in data)
        {
            hash = hash * 127 + num; // Multiply by a prime and add the value
        }
        return hash;
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
