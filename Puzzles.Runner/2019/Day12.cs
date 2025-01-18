using System.Text.RegularExpressions;

namespace Puzzles.Runner._2019;

[Puzzle("The N-Body Problem", 12, 2019)]
public partial class Day12(ILinesInputReader input) : IPuzzleSolver
{
    private const int M16 = 0xFFFF;

    private const int DEMENSIONS = 3;
    private const int MOONS = 4;

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
        var (pos, vel) = Simulate(1000);
        return Energy(pos, vel).ToString();
    }

    public string SolvePart2()
    {
        var periods = Enumerable.Range(0, DEMENSIONS)
            .Select(d => Task.Run(() => FindPeriod(d)))
            .ToArray();

        Task.WhenAll(periods);
        return periods.Select(p => p.Result).LCM().ToString();
    }

    #region Private methods

    private int FindPeriod(int d) 
    {
        var period = 0;

        var simulation = Simulate(d, Int32.MaxValue).GetEnumerator();
        for (int step = 1; simulation.MoveNext(); step++)
        {            
            if (simulation.Current.vel == 0 && simulation.Current.pos == _pos[d])
            {
                period = step;
                break;
            }
        }

        return period;
    }

    private IEnumerable<(long pos, long vel)> Simulate(int d, int count)
    {
        var pos = _pos[d];
        var vel = 0L;

        for(int i = 0; i < count; i++)
        {
            for (int m1 = 0; m1 < MOONS; m1++)
            {
                var dv = 0;

                for (int m2 = 0; m2 < MOONS; m2++)
                    dv += Math.Sign(V(pos, m2) - V(pos, m1));

                vel = Add(vel, Shift((short)dv, m1));
            }

            pos = Add(pos, vel);
            yield return (pos, vel);
        }
    }

    private (long[] pos, long[] vel) Simulate(int count)
    {
        var sim = Enumerable.Range(0, DEMENSIONS)
            .Select(d => Task.Run(() => Simulate(d, count).Last()))
            .ToArray();

        Task.WhenAll(sim);
        return ([..sim.Select(s => s.Result.pos)], [..sim.Select(s => s.Result.vel)]);
    }

    private static long Add(long a, long b)
    {
        var result = 0L;
        for (int m = 0; m < MOONS; m++)
            result |= Shift((short)(V(a, m) + V(b, m)), m);

        return result;
    }

    private static long Shift(short val, int idx)
        => (long)(val & M16) << (0x10 * idx);

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
