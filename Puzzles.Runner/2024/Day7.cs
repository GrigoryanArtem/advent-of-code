namespace Puzzles.Runner._2024;

using OpFunc = Func<ulong, ulong, ulong>;

[Puzzle("Bridge Repair", 7, 2024)]
public class Day7(ILinesInputReader input) : IPuzzleSolver
{
    private const int NUMBER_OF_TASKS = 128;

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
        => CalculateSum([Add, Mult]).ToString();

    public string SolvePart2()
        => CalculateSum([Add, Mult, Concat]).ToString();

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
            if (Find(_input[i], operations, 0, 0, answer, Add))
                sum += answer;
        }

        return sum;
    }

    private static bool Find(ulong[] arr, OpFunc[] operations, int index, ulong acc, ulong answer, OpFunc operation)
    {
        if (acc > answer)
            return false;

        if (index >= arr.Length)
            return acc == answer;

        var value = operation(acc, arr[index]);

        var result = false;
        for (int i = 0; !result && i < operations.Length; i++)
            result |= Find(arr, operations, index + 1, value, answer, operations[i]);

        return result;
    }

    #region Operations

    private static ulong Mult(ulong a, ulong b)
        => a * b;

    private static ulong Add(ulong a, ulong b)
        => a + b;

    private static ulong Concat(ulong a, ulong b)
        => a * (ulong)Math.Pow(10, (ulong)Math.Log10(b) + 1UL) + b;

    #endregion
    #endregion
}
