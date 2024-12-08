namespace Puzzles.Runner._2024;

[Puzzle("Bridge Repair", 7, 2024)]
public class Day7(ILinesInputReader input) : IPuzzleSolver
{
    private const int NUMBER_OF_TASKS = 128;

    private delegate bool Operation(ulong a, ulong b, out ulong result);

    private ulong[] _answers = [];
    private ulong[][] _input = [];

    public void Init()
    {
        _answers = new ulong[input.Lines.Length];
        _input = new ulong[input.Lines.Length][];

        foreach (var (item, idx) in input.Lines.WithIndex())
        {
            var col = item.IndexOf(':');

            _answers[idx] = Convert.ToUInt64(item[..col]);
            _input[idx] = item[(col+1)..].Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(s => Convert.ToUInt64(s.Trim()))
                .ToArray();
        }
    }

    public string SolvePart1()
        => CalculateSum([Div, Sub]).ToString();

    public string SolvePart2()
        => CalculateSum([Split, Div, Sub]).ToString();

    #region Private methods

    private ulong CalculateSum(Operation[] operations)
    {
        var chunkSize = (int)Math.Ceiling((double)_input.Length / NUMBER_OF_TASKS);
        var tasks = Enumerable.Range(0, NUMBER_OF_TASKS)
            .Select(i => CalculateSumAsync(operations, i * chunkSize, chunkSize))
            .ToArray();

        Task.WaitAll(tasks);
        return tasks.Aggregate(0UL, (acc, t) => acc += t.Result);
    }

    private Task<ulong> CalculateSumAsync(Operation[] operations, int start, int count)
        => Task.Run(() => CalculateSum(operations, start, count));

    private ulong CalculateSum(Operation[] operations, int start, int count)
    {
        var end = Math.Min(start + count, _input.Length);
        var sum = 0UL;

        for (int i = start; i < end; i++)
            if (BackFind(_input[i], operations, _answers[i]))
                sum += _answers[i];

        return sum;
    }

    private static bool BackFind(ulong[] arr, Operation[] operations, ulong result)
    {
        var stack = new Stack<(int index, ulong acc)>();
        stack.Push((arr.Length - 1, result));

        while (stack.Count > 0)
        {
            var (index, acc) = stack.Pop();

            if (index == 0)
            {
                if (acc == arr[0])
                    return true;

                continue;
            }

            for (int i = 0; i < operations.Length; i++)
                if (operations[i](acc, arr[index], out var value))
                    stack.Push((index - 1, value));
        }

        return false;
    }

    #region Operations

    private static bool Div(ulong a, ulong b, out ulong result)
    {
        result = a / b;
        return a % b == 0;
    }        

    private static bool Sub(ulong a, ulong b, out ulong result)
    {
        result = a - b;
        return result > 0;
    }

    private static bool Split(ulong a, ulong b, out ulong result)
    {        
        var div = (ulong)Math.Pow(10, (ulong)Math.Log10(b) + 1UL);

        result = a / div;
        return a % div == b;
    }

    #endregion
    #endregion
}
