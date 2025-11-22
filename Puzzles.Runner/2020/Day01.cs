namespace Puzzles.Runner._2020;

[Puzzle("Report Repair", 1, 2020)]
public class Day01(ILinesInputReader input) : IPuzzleSolver
{
    private const int TARGET = 2020;

    private ulong[] _input = [];

    public void Init()
    {
        _input = [..input.Lines.Select(UInt64.Parse)];
    }

    public string SolvePart1()
    {
        HashSet<ulong> nums = [.. _input];

        foreach(var num in _input)
        {
            var x = TARGET - num;
            if (x != num && nums.Contains(x))
                return (x * num).ToString();
        }

        return "";
    }

    public string SolvePart2()
    {
        HashSet<ulong> nums = [.. _input];

        for(var i = 0; i < _input.Length; i++)
        {
            for(int k = i + 1; k < _input.Length; k++)
            {
                var sum = _input[k] + _input[i];
                var x = TARGET - sum;
                if (nums.Contains(x))
                    return (x * _input[k] * _input[i]).ToString();
            }
        }

        return "";
    }
}
