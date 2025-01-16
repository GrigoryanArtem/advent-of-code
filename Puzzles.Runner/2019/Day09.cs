using Puzzles.Runner._2019.Common;

namespace Puzzles.Runner._2019;

[Puzzle("Sensor Boost", 9, 2019)]
public class Day09(ILinesInputReader input) : IPuzzleSolver
{
    private IntCodeMachine _machine;

    public void Init()
        => _machine = new IntCodeMachine(input.GetTokens(",", Convert.ToInt64).First(), UInt16.MaxValue);

    public string SolvePart1()
        => Run(1);

    public string SolvePart2()
        => Run(2);

    public string Run(int input)
    {
        _machine.Reset([input]);
        _machine.Run();

        return String.Join(",", _machine.Output).ToString();
    }
}
