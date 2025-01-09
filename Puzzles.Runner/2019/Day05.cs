using Puzzles.Runner._2019.Common;

namespace Puzzles.Runner._2019;

[Puzzle("Sunny with a Chance of Asteroids", 5, 2019)]
public class Day05(ILinesInputReader input) : IPuzzleSolver
{
    private IntCodeMachine _machine;

    public void Init()
        => _machine = new([.. input.GetTokens(",", Convert.ToInt32).First()]);

    public string SolvePart1()
    {
        _machine.Reset([1]);
        _machine.Run();

        return _machine.Output.Last().ToString();
    }
}
