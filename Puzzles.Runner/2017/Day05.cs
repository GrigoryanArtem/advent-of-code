namespace Puzzles.Runner._2017;

[Puzzle("A Maze of Twisty Trampolines, All Alike", 5, 2017)]
public class Day05(ILinesInputReader input) : IPuzzleSolver
{
    private const int TRESHOLD = 3;

    private int[] _instructions = [];
    private int[] _buffer = [];

    public void Init()
    {
        _instructions = input.Convert(Convert.ToInt32);
        _buffer = new int[_instructions.Length];
    }

    public string SolvePart1()
        => Simulate(1).ToString();    

    public string SolvePart2()
        => Simulate(-1).ToString();

    public int Simulate(int tresholdDelta)
    {
        Array.Copy(_instructions, _buffer, _instructions.Length);

        var step = 0;
        for (uint ptr = 0; ptr < _buffer.Length; step++)
        {
            var jump = _buffer[ptr];

            _buffer[ptr] += jump >= TRESHOLD
                ? tresholdDelta 
                : 1;

            ptr = (uint)(ptr + jump);
        }

        return step;
    }
}
