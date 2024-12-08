namespace Puzzles.Runner._2024;

[Puzzle("Bridge Repair", 7, 2024)]
public class Day7(ILinesInputReader input) : IPuzzleSolver
{
    private const int NUMBER_OF_TASKS = 64;

    private delegate bool OpFunc(ulong a, ulong b, out ulong result);

    ulong[] _answers = [];
    ulong[][] _input = [];

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

    private ulong CalculateSum(OpFunc[] operations)
    {
        var chunkSize = (int)Math.Ceiling((double)_input.Length / NUMBER_OF_TASKS);
        var tasks = Enumerable.Range(0, NUMBER_OF_TASKS)
            .Select(i => CalculateSumAsync(operations, i * chunkSize, chunkSize))
            .ToArray();

        Task.WaitAll(tasks);
        var sum = tasks.Aggregate(0UL, (acc, t) => acc += t.Result);

        return sum;
    }

    private Task<ulong> CalculateSumAsync(OpFunc[] operations, int start, int count)
        => Task.Run(() => CalculateSum(operations, start, count));

    private ulong CalculateSum(OpFunc[] operations, int start, int count)
    {
        ulong sum = 0;
        var end = Math.Min(start + count, _input.Length);

        for (int i = start; i < end; i++)
        {
            var answer = _answers[i];
            if (CheckOperations(_input[i], operations, _input[i].Length - 1, answer))
                sum += answer;
        }

        return sum;
    }

    private static bool BackFind(ulong[] arr, OpFunc[] operations, int index, ulong acc, OpFunc operation)
    {
        if(index == 0)
            return acc == arr[0];

        var success = operation(acc, arr[index], out var value);

        if (!success)
            return false;

        return CheckOperations(arr, operations, index - 1, value);
    }

    private static bool CheckOperations(ulong[] arr, OpFunc[] operations, int index, ulong acc)
    {
        var result = false;
        for (int i = 0; !result && i < operations.Length; i++)
            result |= BackFind(arr, operations, index, acc, operations[i]);

        return result;
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
