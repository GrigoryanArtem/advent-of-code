using Puzzles.Runner._2019.Common;

namespace Puzzles.Runner._2019;

[Puzzle("Care Package", 13, 2019)]
internal class Day13(ILinesInputReader input) : IPuzzleSolver
{
    #region Constants

    private const int CHUNK_SIZE = 3;

    private const int X = 0;
    private const int Y = 1;
    private const int DATA = 2;

    private const int BLOCK = 2;
    private const int PADDLE = 3;
    private const int BALL = 4;

    #endregion

    private IntCodeMachine _machine = IntCodeMachine.Null;

    public void Init()
        => _machine = IntCodeMachine.FromInput(input, UInt16.MaxValue);

    public string SolvePart1()
    {
        _machine.Reset();
        _machine.Run();

        return _machine.Output.Chunk(CHUNK_SIZE)
            .Count(t => t[DATA] == BLOCK)
            .ToString();
    }

    public string SolvePart2()
    {
        _machine.Reset();
        _machine[0] = 2;

        var score = 0L;
        var paddleX = 0L;
        var ballX = 0L;

        while (!_machine.Halted)
        {
            _machine.CleanOutput();
            _machine.Run();

            _machine.Output.Chunk(CHUNK_SIZE).ForEach(chunk =>
            {
                if (chunk[X] == -1 && chunk[Y] == 0)
                {
                    score = chunk[DATA];
                }
                else if (chunk[DATA] == PADDLE)
                {
                    paddleX = chunk[X];
                }
                else if (chunk[DATA] == BALL)
                {
                    ballX = chunk[X];
                }
            });

            _machine.Input(Math.Sign(ballX - paddleX));
        }

        return score.ToString();
    }
}
