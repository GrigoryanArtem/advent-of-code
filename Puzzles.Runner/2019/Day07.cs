using Puzzles.Runner._2019.Common;

namespace Puzzles.Runner._2019;

[Puzzle("Amplification Circuit", 7, 2019)]
public class Day07(ILinesInputReader input) : IPuzzleSolver
{
    private const int AMPLIFIERS_COUNT = 5;
    private int[] _program = [];

    public void Init()
        => _program = [.. input.GetTokens(",", Convert.ToInt32).First()];

    public string SolvePart1()
        => PhaseSettings([0, 1, 2, 3, 4]).Max(ps => Run(ps, loop: false)).ToString();

    public string SolvePart2()
        => PhaseSettings([5, 6, 7, 8, 9]).Max(ps => Run(ps, loop: true)).ToString();

    #region Private methods

    private int Run(int[] phaseSetting, bool loop)
    {
        var amps = Enumerable.Range(0, AMPLIFIERS_COUNT)
            .Select(_ => new IntCodeMachine(_program))
            .ToArray();

        foreach (var (amp, idx) in amps.WithIndex())
            amp.Input(phaseSetting[idx]);

        var output = 0;
        do
        {
            foreach (var amp in amps.Where(a => !a.Halted))
            {
                amp.Input(output);
                amp.Run();

                output = amp.Output.Last();
            }
        } while (loop && !amps.All(a => a.Halted));

        return output;
    }

    private static IEnumerable<int[]> PhaseSettings(int[] src)
        => PhaseSettings(src, 0);

    private static IEnumerable<int[]> PhaseSettings(int[] src, int idx)
    {
        if (idx == src.Length)
            yield return src.ToArray();

        for (var j = idx; j < src.Length; j++)
        {
            (src[idx], src[j]) = (src[j], src[idx]);

            foreach (var perm in PhaseSettings(src, idx + 1))
                yield return perm;

            (src[idx], src[j]) = (src[j], src[idx]);
        }
    }

    #endregion
}
