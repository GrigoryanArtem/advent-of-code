using Puzzles.Runner._2019.Common;

namespace Puzzles.Runner._2019;

[Puzzle("Amplification Circuit", 7, 2019)]
public class Day07(ILinesInputReader input) : IPuzzleSolver
{
    private IntCodeMachine _machine;

    public void Init()
        => _machine = new([.. input.GetTokens(",", Convert.ToInt32).First()]);

    public string SolvePart1()
        => PhaseSettings([0, 1, 2, 3, 4]).Max(Run).ToString();

    private int Run(int[] phaseSetting)
    {
        var output = 0;

        for (int i = 0; i < 5; i++)
        {
            _machine.Reset([phaseSetting[i], output]);
            _machine.Run();

            output = _machine.Output.Last();
        }

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
}

