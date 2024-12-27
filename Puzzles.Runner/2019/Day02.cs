using Puzzles.Runner._2019.Common;

namespace Puzzles.Runner._2019;

[Puzzle("1202 Program Alarm", 2, 2019)]
public class Day02(ILinesInputReader input) : IPuzzleSolver
{
    private const int EXPECTED_RESULT = 19690720;
    private IntCodeMachine _machine;

    public void Init()
        => _machine = new([.. input.GetTokens(",", Convert.ToInt32).First()]);

    public string SolvePart1()
    {        
        _machine.Reset(noun: 12, verb: 2);
        _machine.Run();

        return _machine.Result.ToString();
    }

    public string SolvePart2()
    {        
        for (int noun = 0; noun <= 99; noun++)
        {
            for (int verb = 0; verb <= 99; verb++)
            {
                _machine.Reset(noun, verb);
                _machine.Run();

                if (_machine.Result == EXPECTED_RESULT)
                    return (100 * noun + verb).ToString();
            }
        }

        return String.Empty;
    }    
}
