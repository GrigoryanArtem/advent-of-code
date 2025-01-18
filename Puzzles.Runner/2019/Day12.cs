using System.Text.RegularExpressions;

namespace Puzzles.Runner._2019;

[Puzzle("The N-Body Problem", 12, 2019)]
public partial class Day12(ILinesInputReader input) : IPuzzleSolver
{
    private const int DEMENSIONS = 3;
    private const int MOONS = 4;
    private const int MASK_16 = 0xFFFF;

    private long[] _pos = [];

    public void Init()
    {
        var moons = input.Lines.Select(line => M2V3(MoonRegex().Match(line))).ToArray();

        _pos = new long[DEMENSIONS];
        for (int m = 0; m < moons.Length; m++)
            for (int d = 0; d < DEMENSIONS; d++)
                _pos[d] |= Shift(moons[m][d], m);
    }

    public string SolvePart1()
    {
        var (moons, velocities) = Simulate().ElementAt(999);
        return Energy(moons, velocities).ToString();
    }
    public string SolvePart2()
    {
        var simulation = Simulate().GetEnumerator();
        var periods = new int[DEMENSIONS];

        for (int step = 1; simulation.MoveNext() && !periods.All(p => p > 0); step++)
        {
            var (pos, vel) = simulation.Current;
            for (int d = 0; d < periods.Length; d++)
            {
                if (periods[d] > 0)
                    continue;

                if (vel[d] == 0 && pos[d] == _pos[d])
                    periods[d] = step;
            }
        }

        return periods.LCM().ToString();
    }

    #region Private methods

    private IEnumerable<(long[] pos, long[] vel)> Simulate()
    {
        var pos = _pos.ToArray();
        var vel = new long[DEMENSIONS];

        while (true)
        {
            for (int d = 0; d < DEMENSIONS; d++)
            {
                for (int m1 = 0; m1 < MOONS; m1++)
                {
                    var dv = 0;

                    for (int m2 = 0; m2 < MOONS; m2++)
                        dv += Math.Sign(V(pos[d], m2) - V(pos[d], m1));

                    vel[d] = Add(vel[d], Shift((short)dv, m1));
                }
            }

            for (int d = 0; d < DEMENSIONS; d++)
                pos[d] = Add(pos[d], vel[d]);

            yield return (pos, vel);
        }
    }

    private static long Add(long a, long b)
    {
        var result = 0L;
        for (int m = 0; m < MOONS; m++)
            result |= Shift((short)(V(a, m) + V(b, m)), m);

        return result;
    }

    private static long Shift(short val, int idx)
        => (long)(val & MASK_16) << (0x10 * idx);

    private static long V(long pack, int idx)
        => (short)((pack >> (0x10 * idx)) & 0xFFFF);

    private static long Energy(long[] pos, long[] vel)
        => Enumerable.Range(0, MOONS).Sum(m => Value(pos, m) * Value(vel, m));

    private static long Value(long[] vec, int idx)
        => Enumerable.Range(0, DEMENSIONS).Sum(i => Math.Abs(V(vec[i], idx)));

    private static short[] M2V3(Match match)
        => [M2I32(match, "x"), M2I32(match, "y"), M2I32(match, "z")];

    private static short M2I32(Match match, string group)
        => Convert.ToInt16(match.Groups[group].Value);

    [GeneratedRegex(@"<x=(?<x>-?\d+),\s+y=(?<y>-?\d+),\s+z=(?<z>-?\d+)>")]
    private static partial Regex MoonRegex();

    #endregion
}
