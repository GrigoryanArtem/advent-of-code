using System.Text.RegularExpressions;

namespace Puzzles.Runner._2019;

[Puzzle("The N-Body Problem", 12, 2019)]
public partial class Day12(ILinesInputReader input) : IPuzzleSolver
{    
    private Vec3[] _moons = [];

    public void Init()
    {
        _moons = input.Lines.Select(line => MoonRegex().Match(line))
            .Select(match => new Vec3
            (
                x: Convert.ToInt32(match.Groups["x"].Value),
                y: Convert.ToInt32(match.Groups["y"].Value),
                z: Convert.ToInt32(match.Groups["z"].Value)
            ))
            .ToArray();
    }

    public string SolvePart1()
    {
        var (moons, velocities)= Simulate(1000);
        var energy = Energy(moons, velocities);

        return energy.ToString();
    }

    private (Vec3[] moons, Vec3[] velocities) Simulate(int steps)
    {
        var moons = _moons.ToArray();
        var velocities = new Vec3[moons.Length];

        for (var step = 0; step < steps; step++)
        {
            foreach (var (from, index) in moons.WithIndex())
            {
                foreach (var (to, otherIndex) in moons.WithIndex())
                {
                    if (index == otherIndex)
                        continue;

                    velocities[index].X += Math.Sign(to.X - from.X);
                    velocities[index].Y += Math.Sign(to.Y - from.Y);
                    velocities[index].Z += Math.Sign(to.Z - from.Z);
                }
            }

            for (int i = 0; i < moons.Length; i++)
                moons[i] += velocities[i];
        }

        return (moons, velocities);
    }

    private static int Energy(Vec3[] moons, Vec3[] velocities)
        => moons.Zip(velocities, (m, v) => Value(m) * Value(v)).Sum();

    [GeneratedRegex(@"<x=(?<x>-?\d+),\s+y=(?<y>-?\d+),\s+z=(?<z>-?\d+)>")]
    private static partial Regex MoonRegex();

    private static int Value(Vec3 vec)
        => Math.Abs(vec.X) + Math.Abs(vec.Y) + Math.Abs(vec.Z);
}
