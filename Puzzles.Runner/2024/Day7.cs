namespace Puzzles.Runner._2024;

[Puzzle("Bridge Repair", 7, 2024)]
public class Day7(ILinesInputReader input) : IPuzzleSolver
{
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
    {
        ulong sum = 0;
        foreach(var (input, index) in _input.WithIndex())
        {
            var answer = _answers[index];
            if (Find2(input, 1, input[0], answer, Add) || 
                Find2(input, 1, input[0], answer, Mult))
                sum += answer;
        }

        return sum.ToString();
    }

    public string SolvePart2()
    {
        ulong sum = 0;
        foreach (var (input, index) in _input.WithIndex())
        {
            var answer = _answers[index];
            if (Find3(input, 1, input[0], answer, Add) ||
                Find3(input, 1, input[0], answer, Mult) ||
                Find3(input, 1, input[0], answer, Concat))

                sum += answer;
        }

        return sum.ToString();
    }

    public static bool Find2(ulong[] arr, int index, ulong acc, ulong answer, Func<ulong, ulong, ulong> operation)
    {
        if (acc > answer)
            return false;

        if (index >= arr.Length)
            return acc == answer;

        var value = operation(acc, arr[index]);

        return Find2(arr, index + 1, value, answer, Add) ||
            Find2(arr, index + 1, value, answer, Mult);
    }

    public static bool Find3(ulong[] arr, int index, ulong acc, ulong answer, Func<ulong, ulong, ulong> operation)
    {
        if (acc > answer)
            return false;

        if(index >= arr.Length)
            return acc == answer;

        var value = operation(acc, arr[index]);

        return Find3(arr, index + 1, value, answer, Add) ||
            Find3(arr, index + 1, value, answer, Mult) ||
            Find3(arr, index + 1, value, answer, Concat);
    }

    public static ulong Mult(ulong a, ulong b)
        => a * b;

    public static ulong Add(ulong a, ulong b)
        => a + b;

    public static ulong Concat(ulong a, ulong b)
        => a * (ulong)Math.Pow(10, (ulong)Math.Log10(b) + 1UL) + b;
    
}
